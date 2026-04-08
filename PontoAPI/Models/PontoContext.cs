using Microsoft.EntityFrameworkCore;

namespace PontoAPI.Models;

public class PontoContext : DbContext
{
    public PontoContext(DbContextOptions<PontoContext> options) : base(options) { }
    public DbSet<PontoRegistro> Registros { get; set; } = null!;
}