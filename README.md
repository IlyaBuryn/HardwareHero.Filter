# HardwareHero.Filter

A small library that allows you to create your own filters for collections of the IQueryable type in the form of requests from the client.

## Usage

#### v1.1.0

Expressions that were created in extension methods are moved directly to SortByRequestInfo and GroupByRequestInfo objects.

The GroupBy operation now contains a grouping transformation to an IQueryable. To use your pattern for transformation you need to call `AddGroupByTransformation` method inside your filter.

You can also still use transformation of collection objects without performing grouping, but now a delegate is used instead of the SetSelectionPattern method:

```csharp
public Func<T?, T?> TransformationPattern { get; set; }
```

If the source and filter arguments in the extension methods are null, a `FilterException` will be thrown. In all other cases of error, the exception will be placed in QueryableResponse.

#### v1.0.5

#### Selection

Lets assume that we are using a backend server and some frontend client that sends HTTP requests to the server.

To demonstrate the usage, I will specify that our server has one main model available with references to other models:

```csharp
public class Component
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ComponentTypeId { get; set; }

    public virtual ComponentType? ComponentType { get; set; }
    public virtual ICollection<ComponentImages>? ComponentImages { get; set; }
    public virtual ICollection<ComponentAttributes>? ComponentAttributes { get; set; }
}
```

The whole idea of the filter is that when a request is submitted, the client will know what the filter looks like and what data should be transferred to the server.

Therefore, a new class can be created to denote the filter, which will implement the abstract class `FilterRequestDomain<T>`. In our case:

```csharp
  public class ComponentsFilter : FilterRequestDomain<Component>
```

And in this class we can specify which fields should be in the request, for example:

```csharp
  public string? SearchString { get; set; }
  public string? Type { get; set; }
```

Of course, you can add anything to this class, but in the current case I will specify only those fields that will be used for some operations.

Now, if we want to use some selection based on these fields, we need to implement a basic constructor inside which call the `AddExpression(x)` method where _x_ is a lambda expression specifying a rule for the selection. For example:

```csharp
  public ComponentsFilter() : base()
  {
      AddExpression(x => x.Name.Contains(SearchString) || x.Description.Contains(SearchString));
      AddExpression(x => x.ComponentType.Name == Type || x.ComponentType.FullName == Type);
  }
```

Now in the class where you will apply this filter, you get a collection from the database and call the `ApplyFilter(filter)` extension method for the resulting collection, where _filter_ is the object of our particular filter. It's important to clarify that extension methods are only available for collections of type `IQueryable<T?>`. In my case it would look like this:

```csharp
var query = await _componentRepo.GetManyEntitiesAsync();
query = query.ApplyFilter(filter).Query;
```

All extension methods will return a single object of type `QueryableResponse<T>` where you can call the `Query` property or check for errors received while executing extension methods. I would like to note that since version `1.0.4` all extension methods almost always return a non-empty Query even if errors are detected.

#### OrderBy, GroupBy, SelectionPattern and Pagination

Besides the main task of the filter, there are some other methods and classes that may be useful. For example, there are such extension methods for IQueryable as:

- ApplyOrderBy
- ApplyGroupBy
- ApplyPagination
- ApplySelection

There are three properties in the `FilterRequestDomain` class for the first three methods to work:

```csharp
  public PageRequestInfo? PageRequestInfo { get; set; }
  public SortByRequestInfo? SortByRequestInfo { get; set; }
  public GroupByRequestInfo? GroupByRequestInfo { get; set; }
```

Nothing unusual: these objects contain properties appropriate to their actions. And the appropriate extension methods are used for these properties. However, the `ApplyGroupBy` and `ApplySelection` methods may need special methods from the filter base class that can be overridden. These are the `GroupedPattern` and `SelectionPattern` methods accordingly.

- `GroupedPattern` allows you to explicitly specify which pattern will be sampled after grouping. By default this method simply excludes duplicates, but for complex data different constructs can be applied. For example, in one of my projects I have tables storing key/values and for a nice selection I redefined the method in this way:

```csharp
public override IQueryable<ComponentAttributes?>? GroupedPattern(IQueryable<IGrouping<object, ComponentAttributes?>> groups)
{
    var query = groups.Select(group => new ComponentAttributes
    {
        AttributeName = (string)(group.Key is string ? group.Key : string.Empty),
        AttributeValue = string.Join("|", group.Select(attr => attr.AttributeValue).Distinct().ToList()),
    });

    return query;
}
```

_It doesn't look great, but it does the job_

- `SelectionPattern` is a method that restricts or extends the properties of a collection object from a selection. By default it returns the object itself.
  What is the purpose of this method? In my case, I was able to limit the properties of the particular Component object. Since each such object refers to many other objects, these references can be removed in this way:

```csharp
public override Component SelectionPattern(Component refItem)
{
    if (refItem.ComponentImages != null)
    {
        refItem.ComponentImages = refItem.ComponentImages.Select(ci => new ComponentImages
        {
            Id = ci.Id,
            Image = ci.Image,
            Component = null
        }).ToList();
    }

    return new Component
    {
        Id = refItem.Id,
        Name = refItem.Name,
        Description = refItem.Description,
        ComponentTypeId = refItem.ComponentTypeId,
        ComponentImages = refItem.ComponentImages,
    };
}
```