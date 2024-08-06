using AutoMapper;
using manto_stock_system_API.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace manto_stock_system_API.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<ListResponse<TDTO>> FilterSortPaginate<TEntity, TDTO>(
            this IQueryable<TEntity> queryable,
            BaseFilter baseFilter,
            IMapper mapper) where TEntity : class, new()
        {
            queryable = queryable.Filter<TEntity>(baseFilter.Filters);

            var count = await queryable.CountAsync();

            queryable = queryable.Sort(baseFilter.Sort);

            queryable = queryable.Paginate(baseFilter.Range);

            return new ListResponse<TDTO>(mapper.Map<List<TDTO>>(queryable), count);
        }

        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> queryable, string filtersString) where TEntity : class, new()
        {
            if (String.IsNullOrEmpty(filtersString)) return queryable;

            var source = new TEntity();

            // See comment on DTOs/Filters/BaseFilter.cs line 8
            var filters = JsonConvert.DeserializeObject<List<FilterValue>>(filtersString);
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var property = source.GetType().GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                    string query = $"{filter.Field} == @0"; // Basic query

                    if (property != null)
                    {
                        if (property.PropertyType == typeof(string)) query = $"{filter.Field}.ToLower().Contains(@0.ToLower())";
                    }
                    else
                    {
                        if (filter.Field.Contains("_OR")) // OR filtering
                        {
                            var key = filter.Field.Replace("_OR", "");
                            var values = filter.Value.Split(',');
                            query = $"e => ";
                            foreach (var value in values)
                            {
                                if (values.First() != value) query += " || "; // Avoiding adding || on first loop
                                query += $"e.{key} == {value}";
                            }
                            queryable = queryable.Where(query);
                        }
                        else if (filter.Field.Contains("_str")) // Property is a string
                        {
                            var key = filter.Field.Replace("_str", "");
                            query = $"e => e.{key}.Contains(@0)";
                        }
                        else if (filter.Field.Contains("_bgr")) // Element property must be bigger or equal than filter value
                        {
                            var key = filter.Field.Replace("_bgr", "");
                            query = $"e => e.{key} >= @0";
                        }
                        else if (filter.Field.Contains("_sml")) // Element property value must be smaller or equal than filter value
                        {
                            var key = filter.Field.Replace("_sml", "");
                            query = $"e => e.{key} <= @0";
                        }
                        else if (filter.Field.Contains("_cnt")) // Element property value must contain filter value
                        {
                            var key = filter.Field.Replace("_cnt", "");
                            query = $"e => e.{key}.ToString().Contains(@0.ToString())";
                        }
                    }
                    try
                    {
                        // When filtering in parallel (OR) the queryable is already filtered at this point
                        if (!filter.Field.Contains("_OR")) queryable = queryable.Where(query, SanitizeFilterValue(filter.Value));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return queryable;
        }

        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> queryable, Sort sort)
        {
            if (sort == null) return queryable;
            var order = sort.IsAscending ? "" : "descending";
            return queryable.OrderBy($"{sort.Field} {order}");
        }

        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> queryable, DTOs.Range range)
        {
            return queryable.Skip(range.Start).Take(range.End - range.Start + 1);
        }

        private static string SanitizeFilterValue(JToken value)
        {
            return value.ToString().Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("\"", "").Trim();
        }
    }
}
