
namespace MapController.Persistence
{
    public interface IMapCommandDataAccess
    {
        void DeleteMap(int id);
        List<Map> GetMaps();
        void InsertMaps(Map map);
        void UpdateMap(int id, Map map);
    }
}