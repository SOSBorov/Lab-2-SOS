using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

public class AppDbContext : DbContext
{
	public AppDbContext()
	{
	}

	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	public DbSet<TodoItem> Todos => Set<TodoItem>();
	public DbSet<Profile> Profiles => Set<Profile>();
	public DbSet<User> Users => Set<User>();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.IsConfigured)
		{
			return;
		}

		string dbPath = Path.Combine(AppContext.BaseDirectory, "todos.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Profile>(entity =>
		{
			entity.HasKey(profile => profile.Id);
			entity.Property(profile => profile.Id).ValueGeneratedNever();

			entity.Property(profile => profile.Login)
				.IsRequired()
				.HasMaxLength(50);

			entity.Property(profile => profile.Password)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(profile => profile.FirstName)
				.IsRequired()
				.HasMaxLength(50);

			entity.Property(profile => profile.LastName)
				.HasMaxLength(50);

			entity.Property(profile => profile.BirthYear)
				.IsRequired();

			entity.Ignore(profile => profile.Info);

			entity.HasMany(profile => profile.TodoItems)
				.WithOne(todo => todo.Profile)
				.HasForeignKey(todo => todo.ProfileId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.ToTable(table =>
			{
				table.HasCheckConstraint("CK_Profile_Login_NotEmpty", "length(trim(Login)) >= 1");
				table.HasCheckConstraint("CK_Profile_Password_NotEmpty", "length(trim(Password)) >= 1");
				table.HasCheckConstraint("CK_Profile_FirstName_NotEmpty", "length(trim(FirstName)) >= 1");
				table.HasCheckConstraint("CK_Profile_BirthYear_Range", "BirthYear >= 1900 AND BirthYear <= 2100");
			});
		});

		modelBuilder.Entity<TodoItem>(entity =>
		{
			entity.HasKey(todo => todo.Id);
			entity.Property(todo => todo.Id).ValueGeneratedNever();

			entity.Property(todo => todo.Text)
				.IsRequired()
				.HasMaxLength(1000);

			entity.Property(todo => todo.Status)
				.IsRequired();

			entity.Property(todo => todo.CreatedAt)
				.IsRequired();

			entity.Property(todo => todo.LastUpdated)
				.IsRequired();

			entity.Property(todo => todo.ProfileId)
				.IsRequired();

			entity.ToTable(table =>
			{
				table.HasCheckConstraint("CK_TodoItem_Text_NotEmpty", "length(trim(Text)) >= 1");
			});
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(user => user.Id);
			entity.Property(user => user.Id).ValueGeneratedOnAdd();

			entity.Property(user => user.Username)
				.IsRequired()
				.HasMaxLength(50);

			entity.Property(user => user.Email)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(user => user.PasswordHash)
				.IsRequired()
				.HasMaxLength(500);

			entity.Property(user => user.Role)
				.IsRequired()
				.HasMaxLength(20);

			entity.Property(user => user.ProfileId)
				.IsRequired();

			entity.HasIndex(user => user.Username)
				.IsUnique();

			entity.HasIndex(user => user.Email)
				.IsUnique();

			entity.HasIndex(user => user.ProfileId)
				.IsUnique();

			entity.HasOne<Profile>()
				.WithMany()
				.HasForeignKey(user => user.ProfileId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.ToTable(table =>
			{
				table.HasCheckConstraint("CK_User_Username_NotEmpty", "length(trim(Username)) >= 1");
				table.HasCheckConstraint("CK_User_Email_NotEmpty", "length(trim(Email)) >= 1");
				table.HasCheckConstraint("CK_User_PasswordHash_NotEmpty", "length(trim(PasswordHash)) >= 1");
				table.HasCheckConstraint("CK_User_Role_NotEmpty", "length(trim(Role)) >= 1");
			});
		});
	}
}
