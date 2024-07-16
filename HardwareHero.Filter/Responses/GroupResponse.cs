using HardwareHero.Filter.Exceptions;

namespace HardwareHero.Filter.Responses
{
    public class GroupResponse<T> where T : class
    {
        public List<GroupItem<T>> Groups { get; set; }
        public List<FilterException?> Errors { get; set; } = new();
        public bool IsSuccessful { get; set; }

        public GroupResponse(List<GroupItem<T>> groups)
        {
            Groups = groups;
            Errors = new();
            IsSuccessful = true;
        }

        public GroupResponse(FilterException? exception)
        {
            Groups = new List<GroupItem<T>>();
            Errors.Add(exception);
            IsSuccessful = false;
        }
    }

    public class GroupItem<T>
    {
        public string Key { get; set; }
        public IQueryable<T> Query { get; set; }

        public GroupItem(string key, IQueryable<T> query)
        {
            Key = key;
            Query = query;
        }
    }
}
