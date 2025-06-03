namespace Squares.Persistence
{
    public interface ISquaresDataAccess
    {
        Task<(Table, Player, Player)> LoadData(string path);
        Task<(Table, Player, Player)> LoadData(Stream path);
        Task SaveData(string path, Table table, Player player01, Player player02);
        Task SaveData(Stream path, Table table, Player player01, Player player02);
    }
}
