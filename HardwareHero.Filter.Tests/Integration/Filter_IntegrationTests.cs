using HardwareHero.Filter.Extensions;
using HardwareHero.Filter.Tests.Data;
using HardwareHero.Filter.Tests.Models;
using System.Diagnostics;
using Xunit.Abstractions;

namespace HardwareHero.Filter.Tests.Integration
{
    public class Filter_IntegrationTests
    {
        private readonly IFixture _fixture;
        private readonly IQueryable<Component> _collection;
        private readonly ITestOutputHelper _output;
        private ComponentFilter _filter;
        private Stopwatch _stopwatch;

        public Filter_IntegrationTests(ITestOutputHelper output)
        {
            _fixture = new Fixture();
            _collection = _fixture.CreateMany<Component>(100).AsQueryable();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _output = output;
        }

        [Fact]
        public void ApplyFilter_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var response = _collection.ApplyFilter(_filter);
            var result = response.Query.ToList();
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(33);
            response.Errors.Should().BeNullOrEmpty();
            response.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplySorting_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var response = _collection.ApplyOrderBy(_filter);
            var result = response.Query.ToList();
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(100);
            result.Should().BeInDescendingOrder(x => x.Id);
            response.Errors.Should().BeNullOrEmpty();
            response.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplyGrouping_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var response = _collection.ApplyGroupBy(_filter);
            var result = response.Groups;
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Key == "beta");
            result.Where(x => x.Key == "beta").FirstOrDefault().Query.Count().Should().Be(67);
            result.Count().Should().BeGreaterThanOrEqualTo(2);
            response.Errors.Should().BeNullOrEmpty();
            response.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplyPagination_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var response = _collection.ApplyPagination(_filter);
            var result = response.QueryableResponse.Query;
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be((int)_filter.PageSize);
            response.CurrentPageNumber.Should().Be(_filter.PageNumber);
            response.CurrentPageSize.Should().Be(_filter.PageSize);
            response.TotalPages.Should().Be(7);
            response.QueryableResponse.Errors.Should().BeNullOrEmpty();
            response.QueryableResponse.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplySelection_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var response = _collection.ApplySelection(_filter);
            var result = response.Query;
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(100);
            result.First().GetType().GetProperties().Where(x => x.Name == "Id").Should().NotBeNull();
            result.First().GetType().GetProperties().Where(x => x.Name == "Name").Should().BeEmpty();
            response.Errors.Should().BeNullOrEmpty();
            response.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplyEverythingExceptGrouping_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var filtered = _collection.ApplyFilter(_filter);
            var sorted = filtered.Query.ApplyOrderBy(_filter);
            var paginated = sorted.Query.ApplyPagination(_filter);
            var selected = paginated.QueryableResponse.Query.ApplySelection(_filter);
            var result = selected.Query;
            var paginatedResult = paginated.QueryableResponse.Query;
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be((int)_filter.PageSize);
            paginatedResult.Count().Should().Be((int)_filter.PageSize);
            paginated.TotalPages.Should().Be(3);
            result.First().GetType().GetProperties().Where(x => x.Name == "Id").Should().NotBeNull();
            result.First().GetType().GetProperties().Where(x => x.Name == "Name").Should().BeEmpty();
            paginated.QueryableResponse.Errors.Should().BeNullOrEmpty();
            selected.Errors.Should().BeNullOrEmpty();
            paginated.QueryableResponse.IsSuccessful.Should().BeTrue();
            selected.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplyEverythingExceptPaginationAndSelection_WithCorrectFilterProps_ShouldFilterCorrectly()
        {
            // Arrange
            SetupAdditionalCollectionData();
            _filter = SetupCorrectComponentFilter();

            // Act
            _stopwatch.Restart();
            var filtered = _collection.ApplyFilter(_filter);
            var sorted = filtered.Query.ApplyOrderBy(_filter);
            var grouped = sorted.Query.ApplyGroupBy(_filter);
            var result = grouped.Groups;
            _stopwatch.Stop();
            _output.WriteLine($"Performance time: {_stopwatch.Elapsed}");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Key == "alpha");
            result.Should().NotContain(x => x.Key == "beta");
            result.Should().NotBeNullOrEmpty();
            grouped.Errors.Should().BeNullOrEmpty();
            grouped.IsSuccessful.Should().BeTrue();
        }

        private ComponentFilter SetupCorrectComponentFilter()
        {
            return new ComponentFilter()
            {
                SearchString = "test",
                Type = "",
                Attributes = new() {
                    { "test", "100" }
                },
                PageNumber = 1,
                PageSize = 15,
                SortByDescending = true,
                GroupByProperty = "Type",
                SortByProperty = "Id"
            };
        }

        private void SetupAdditionalCollectionData()
        {
            int x = 1;
            foreach (var component in _collection)
            {

                if (x % 3 == 0)
                {
                    component.Description = "1test1";
                    component.ComponentAttributes.Add(new ComponentAttribute
                    {
                        Key = "test",
                        Value = "100",
                    });
                    component.ComponentType.Type = "alpha";
                }
                else
                {
                    component.ComponentType.Type = "beta";
                    component.ComponentAttributes.Add(new ComponentAttribute
                    {
                        Key = "test",
                        Value = "90",
                    });
                }
                x++;
            }
        }
    }
}
