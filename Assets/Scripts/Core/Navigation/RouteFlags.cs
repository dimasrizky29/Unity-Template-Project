[System.Flags]
public enum RouteFlags
{
    None = 0,
    RequiresAuth = 1 << 0, // 1
    IsAdditive = 1 << 1, // 2
    IsTransient = 1 << 2, // 4 (rute ini tidak disimpan di history)
    ClearHistory = 1 << 3, // 8 (rute ini menghapus semua history lama)
}