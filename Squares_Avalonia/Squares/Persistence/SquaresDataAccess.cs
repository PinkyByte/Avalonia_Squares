namespace Squares.Persistence
{
    public class SquaresDataAccess : ISquaresDataAccess
    {
        public async Task<(Table, Player, Player)> LoadData(string path)
        {
            return await LoadData(File.OpenRead(path));
        }
        public async Task<(Table, Player, Player)> LoadData(Stream path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = await reader.ReadLineAsync() ?? string.Empty;
                    string[] splitted = line.Split(" ");
                    int player01 = int.Parse(splitted[0]);
                    int player02 = int.Parse(splitted[1]);
                    int n  = int.Parse(splitted[2]);
                    Table table = new Table(n);
                    for (int i = 0; i < table.Rows.GetLength(0); i++)
                    {
                        line = await reader.ReadLineAsync() ?? string.Empty;
                        splitted = line.Split(" ");
                        for (int j = 0; j < table.Rows.GetLength(1); j++)
                        {
                            table.Rows[i, j] = bool.Parse(splitted[j]);
                        }
                    }
                    for (int i = 0; i < table.Columns.GetLength(0); i++)
                    {
                        line = await reader.ReadLineAsync() ?? string.Empty;
                        splitted = line.Split(" ");
                        for (int j = 0; j < table.Columns.GetLength(1); j++)
                        {
                            table.Columns[i, j] = bool.Parse(splitted[j]);
                        }
                    }
                    return (table, new Player(player01, "kék"), new Player(player02, "narancs"));
                }
            }
            catch
            {
                throw new SquaresDataException();
            }
        }
        public async Task SaveData(string path, Table table, Player player01, Player player02)
        {
            await SaveData(File.OpenWrite(path), table, player01, player02);
        }
        public async Task SaveData(Stream path, Table table, Player player01, Player player02)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(player01.Score + " " + player02.Score + " " + (table.Rows.GetLength(0) - 1));
                    for (int i = 0; i < table.Rows.GetLength(0); i++)
                    {
                        for (int j = 0; j < table.Rows.GetLength(1); j++)
                        {
                            await writer.WriteAsync(table.Rows[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                    for (int i = 0; i < table.Columns.GetLength(0); i++)
                    {
                        for (int j = 0; j < table.Columns.GetLength(1); j++)
                        {
                            await writer.WriteAsync(table.Columns[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new SquaresDataException();
            }
        }
    }
}
