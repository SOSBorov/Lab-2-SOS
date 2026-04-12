using Microsoft.EntityFrameworkCore;

namespace TodoList
{
	public class AppDbContext : DbContext
	{
		public DbSet<TodoItem> Todos => Set<TodoItem>();
		public DbSet<Profile> Profiles => Set<Profile>();

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=todos.db");
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
		}
	}
}
