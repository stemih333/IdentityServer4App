using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanTasks.IdentityServer4.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var modifiedEntries = ChangeTracker.Entries()
               .Where(x => x.Entity is ApplicationUser
                   && (x.State == EntityState.Added || x.State == EntityState.Modified)).ToList();

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is ApplicationUser entity)
                {
                    DateTime now = DateTime.Now;

                    if (entry.State == EntityState.Added)
                    {
                        entity.Created = now; //Update 'Created' column on all inserts
                    }
                    else
                    {
                        Entry(entity).Property(x => x.Created).IsModified = false;
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    }

                    entity.Updated = now;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var modifiedEntries = ChangeTracker.Entries()
               .Where(x => x.Entity is ApplicationUser
                   && (x.State == EntityState.Added || x.State == EntityState.Modified)).ToList();

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is ApplicationUser entity)
                {
                    DateTime now = DateTime.Now;

                    if (entry.State == EntityState.Added)
                    {
                        entity.Created = now; //Update 'Created' column on all inserts
                    }
                    else
                    {
                        Entry(entity).Property(x => x.Created).IsModified = false;
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    }

                    entity.Updated = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
               .Where(x => x.Entity is ApplicationUser
                   && (x.State == EntityState.Added || x.State == EntityState.Modified)).ToList();

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is ApplicationUser entity)
                {
                    DateTime now = DateTime.Now;

                    if (entry.State == EntityState.Added)
                    {
                        entity.Created = now; //Update 'Created' column on all inserts
                    }
                    else
                    {
                        Entry(entity).Property(x => x.Created).IsModified = false;
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    }

                    entity.Updated = now;
                }
            }

            return base.SaveChanges();
        }
    }
}
