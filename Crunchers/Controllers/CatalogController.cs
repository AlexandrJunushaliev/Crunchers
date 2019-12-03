using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Crunchers.Models;

namespace Crunchers.Controllers
{
    public class CatalogController : Controller
    {
        // GET
        public async Task<ActionResult> Index()
        {
            var categories = await new CategoryModel().GetCategories();
            return View(categories);
        }

        public class CatalogFiltersResponse
        {
            public bool IsEnabled;
            public string FilterShowValue;
            public string CharacteristicName;
            public int CharacteristicId;
            public int FilterId;
            public bool isRanged;

            public override bool Equals(object obj)
            {
                return obj is CatalogFiltersResponse item && this.FilterShowValue.Equals(item.FilterShowValue);
            }

            public override int GetHashCode()
            {
                return this.FilterShowValue.GetHashCode();
            }
        }

        public class CatalogProductsResponse
        {
            public IEnumerable<IGrouping<string, CatalogFiltersResponse>> Filters;
            public IEnumerable<IGrouping<int, ProductModel>> Products;
            public int MaxPrice;
            public int MinPrice;
        }

        public async Task<ActionResult> CategoryProducts(int categoryId, [FromUri] string[] filterValues = null,
            string moneyFilter = null, bool orderBy = false, bool orderByDescending = false,
            bool ratings = false, bool money = false)
        {
            var filterValuesTuples = new List<Tuple<int, string>>();
            if (filterValues != null)
            {
                filterValuesTuples.AddRange(filterValues.Select(filterValue => filterValue.Split('_'))
                    .Select(pair => pair.Length == 1
                        ? Tuple.Create(int.Parse(pair[0]), "")
                        : Tuple.Create(int.Parse(pair[0]), pair[1])));
            }

            var filterModels = await new FilterModel().GetFiltersByCategoryId(categoryId);
            var products = filterValues != null
                ? await new ProductModel().FilterProducts(filterValuesTuples, filterModels, categoryId)
                : await new ProductModel().GetProductsByCategoryId(categoryId);
            var maxPriceForResponse = -1;
            var minPriceForResponse = int.MaxValue;
            foreach (var product in products)
            {
                maxPriceForResponse = maxPriceForResponse < product.ProductPrice
                    ? product.ProductPrice
                    : maxPriceForResponse;
                minPriceForResponse = minPriceForResponse > product.ProductPrice
                    ? product.ProductPrice
                    : minPriceForResponse;
            }

            var moneyPair = moneyFilter?.Split(':');
            var moneyMin = moneyPair?[0];
            var moneyMax = moneyPair?[1];
            products = moneyPair != null
                ? products.Where(x => x.ProductPrice <= int.Parse(moneyMax) && x.ProductPrice >= int.Parse(moneyMin))
                : products;

            var filters = products.Join(filterModels, x => x.CharacteristicId, y => y.CharacteristicId, (x, y) =>
                {
                    return new
                    {
                        y.From,
                        y.To,
                        y.CharacteristicId,
                        y.CharacteristicName,
                        x.ValueToCharName,
                        y.FilterId
                    };
                })
                .Where(x => x.From < 0 ||
                            x.From >= 0 && x.ValueToCharName.Item2 <= x.To && x.ValueToCharName.Item2 >= x.From).Select(
                    x => new CatalogFiltersResponse()
                    {
                        IsEnabled =
                            filterValues != null && filterValuesTuples.Select(y => y.Item1).Contains(x.FilterId),
                        FilterShowValue = x.From >= 0 ? $"от {x.From} до {x.To}" : $"{x.ValueToCharName.Item2}",
                        CharacteristicName = x.CharacteristicName,
                        CharacteristicId = x.CharacteristicId,
                        FilterId = x.FilterId,
                        isRanged = !(x.From < 0)
                    })
                .Distinct().GroupBy(x => x.CharacteristicName);
            var productsGroups = products.GroupBy(x => x.ProductId);

            productsGroups = orderBy && money ? productsGroups.OrderBy(x => x.First().ProductPrice) :
                orderByDescending && money ? productsGroups.OrderByDescending(x => x.First().ProductPrice) :
                orderBy && ratings ? productsGroups.OrderBy(x => x.First().RatingSum) :
                orderByDescending && ratings ? productsGroups.OrderByDescending(x => x.First().RatingSum) : productsGroups;

            var response = new CatalogProductsResponse()
            {
                Products = productsGroups, Filters = filters, MaxPrice = maxPriceForResponse,
                MinPrice = minPriceForResponse
            };


            return View(response);
        }
    }
}