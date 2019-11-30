using System.Data.Common;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class FilterModel
    {
        public readonly int From;
        public readonly int To;
        public readonly int CharacteristicId;
        public readonly int FilterId;
        public DbCommand _dbCommand;
        public DbConnection _dbConnection;

        public FilterModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public FilterModel(int from, int to, int characteristicId, int filterId)
        {
            From = from;
            To = to;
            CharacteristicId = characteristicId;
            FilterId = filterId;
        }

        public void AddFilter(int characteristicId, int? from, int? to)
        {
            var sqlConnection =
                string.Format(
                    "Insert into \"Filters\"(\"CharacteristicId\",\"From\",\"To\") values ('{0}','{1}','{2}')",
                    characteristicId, from, to);
            
            
        }
    }
}