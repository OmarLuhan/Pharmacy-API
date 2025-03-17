using Microsoft.EntityFrameworkCore;

namespace Farma_api.Models;

public partial class FarmadbContext : DbContext
{
    public FarmadbContext()
    {
    }

    public FarmadbContext(DbContextOptions<FarmadbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Correlativo> Correlativos { get; set; }

    public virtual DbSet<DetalleEntrada> DetalleEntradas { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; }

    public virtual DbSet<Entrada> Entradas { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StockAudit> StockAudits { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Descripcion, "descripcion").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<Correlativo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadDigitos).HasColumnName("cantidadDigitos");
            entity.Property(e => e.FechaActualizacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Gestion)
                .HasMaxLength(50)
                .HasColumnName("gestion");
            entity.Property(e => e.UltimoNumero).HasColumnName("ultimoNumero");
        });

        modelBuilder.Entity<DetalleEntrada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.EntradaId, "entradaId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CodigoProducto)
                .HasMaxLength(50)
                .HasColumnName("codigoProducto");
            entity.Property(e => e.EntradaId).HasColumnName("entradaId");
            entity.Property(e => e.LoteId).HasColumnName("loteId");
            entity.Property(e => e.LoteNumero)
                .HasMaxLength(50)
                .HasColumnName("loteNumero");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(100)
                .HasColumnName("nombreProducto");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Entrada).WithMany(p => p.DetalleEntrada)
                .HasForeignKey(d => d.EntradaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetalleEntradas_ibfk_1");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.VentaId, "ventaId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CodigoProducto)
                .HasMaxLength(50)
                .HasColumnName("codigoProducto");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(100)
                .HasColumnName("nombreProducto");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");

            entity.HasOne(d => d.Venta).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetalleVentas_ibfk_1");
        });

        modelBuilder.Entity<Entrada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.UsuarioId, "usuarioId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Correlativo)
                .HasMaxLength(6)
                .HasColumnName("correlativo");
            entity.Property(e => e.Documento)
                .HasMaxLength(20)
                .HasColumnName("documento");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.ImpuestoTotal)
                .HasPrecision(10, 2)
                .HasColumnName("impuestoTotal");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(50)
                .HasColumnName("proveedor");
            entity.Property(e => e.SubTotal)
                .HasPrecision(10, 2)
                .HasColumnName("subTotal");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Entradas_ibfk_1");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductoId, "productoId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.FechaProduccion).HasColumnName("fechaProduccion");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fechaVencimiento");
            entity.Property(e => e.NumeroLote)
                .HasMaxLength(50)
                .HasColumnName("numeroLote");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.StockLote).HasColumnName("stockLote");

            entity.HasOne(d => d.Producto).WithMany(p => p.Lotes)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("Lotes_ibfk_1");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.CategoriaId, "categoriaId");

            entity.HasIndex(e => e.CodigoEan13, "codigoEAN13").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoriaId");
            entity.Property(e => e.CodigoEan13)
                .HasMaxLength(14)
                .HasColumnName("codigoEAN13");
            entity.Property(e => e.Concentracion)
                .HasMaxLength(100)
                .HasColumnName("concentracion");
            entity.Property(e => e.Especial).HasColumnName("especial");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.Presentacion)
                .HasMaxLength(100)
                .HasColumnName("presentacion");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Productos_ibfk_1");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("timestamp")
                .HasColumnName("fechaExpiracion");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.TokenRefresh)
                .HasMaxLength(200)
                .HasColumnName("tokenRefresh");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Valor)
                .HasMaxLength(20)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<StockAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.LoteId, "loteId");

            entity.HasIndex(e => e.VentaId, "ventaId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadReducida).HasColumnName("cantidadReducida");
            entity.Property(e => e.Fecha)
                .HasColumnType("timestamp")
                .HasColumnName("fecha");
            entity.Property(e => e.LoteId).HasColumnName("loteId");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");

            entity.HasOne(d => d.Lote).WithMany(p => p.StockAudits)
                .HasForeignKey(d => d.LoteId)
                .HasConstraintName("StockAudits_ibfk_2");

            entity.HasOne(d => d.Venta).WithMany(p => p.StockAudits)
                .HasForeignKey(d => d.VentaId)
                .HasConstraintName("StockAudits_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Correo, "correo").IsUnique();

            entity.HasIndex(e => e.RolId, "rolId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Clave)
                .HasMaxLength(500)
                .HasColumnName("clave");
            entity.Property(e => e.Correo).HasColumnName("correo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.RolId).HasColumnName("rolId");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Usuarios_ibfk_1");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.UsuarioId, "usuarioId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteDni)
                .HasMaxLength(12)
                .HasColumnName("clienteDni");
            entity.Property(e => e.ClienteNombre)
                .HasMaxLength(100)
                .HasColumnName("clienteNombre");
            entity.Property(e => e.Correlativo)
                .HasMaxLength(6)
                .HasColumnName("correlativo");
            entity.Property(e => e.Documento)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Boleta'")
                .HasColumnName("documento");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.ImpuestoTotal)
                .HasPrecision(10, 2)
                .HasColumnName("impuestoTotal");
            entity.Property(e => e.SubTotal)
                .HasPrecision(10, 2)
                .HasColumnName("subTotal");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Venta)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Ventas_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}