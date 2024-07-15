using HardwareHero.Filter.Operations;
using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Tests.Models;
using System;
using System.Linq.Expressions;

namespace HardwareHero.Filter.Tests.Data
{
    public class ComponentFilter : FilterRequestDomain<Component>,
        IFilterable<Component>, ISortable<Component>, IPaginable, IGroupable<Component>, ISelectable<Component>
    {
        public ComponentFilter()
        {
            SetupFilterExpressions();
            SetupSortByExpressions();
            SetupGroupByExpressions();
        }

        public string? SearchString { get; set; }
        public string? Type { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string? SortByProperty { get; init; }
        public bool SortByDescending { get; init; } = true;
        public uint PageNumber { get; init; } = 1;
        public uint PageSize { get; init; } = 10;
        public string? GroupByProperty { get; init; }

        public Expression<Func<Component, bool>>? OnGetFilterExpression()
            => GetFilterExpression(nameof(Component));

        public Expression<Func<Component, object>>? OnGetGroupExpression(string groupByProperty)
            => GetGroupExpression(GroupByProperty);

        public Expression<Func<Component, object>>? OnGetSortExpression(string? sortByProperty)
            => GetSortExpression(sortByProperty);

        public void SetupFilterExpressions()
        {
            FilterExpressions[nameof(Component)] = 
                component => 
                (string.IsNullOrEmpty(SearchString) || (component.Name.Contains(SearchString) || component.Description.Contains(SearchString))) &&
                (string.IsNullOrEmpty(Type) || (component.ComponentType != null ? component.ComponentType.Type == Type : true)) &&
                (Attributes.Count != 0 ||
                Attributes.All(attribute =>
                    component.ComponentAttributes.Any(a =>
                        a.Key == attribute.Key &&
                        a.Value.Contains(attribute.Value))));
        }

        public void SetupGroupByExpressions()
        {
            GroupByExpressions[nameof(Component.ComponentType.Type)] = 
                component => component.ComponentType.Type;
        }

        public object SetupSelectFields(Component? item)
        {
            return new
            {
                Id = item.Id
            };
        }

        public void SetupSortByExpressions()
        {
            SortByExpressions[nameof(Component.Id)] = component => component.Id;
        }
    }
}
