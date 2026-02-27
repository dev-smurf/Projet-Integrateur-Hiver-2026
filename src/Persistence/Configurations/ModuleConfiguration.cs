<<<<<<< HEAD
using Domain.Entities;
=======
﻿using Domain.Entities;
>>>>>>> f69b815974f0a4005de560cf3b0e2b257c691b83
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nom)
                .IsRequired();

            builder.Property(m => m.Contenu)
                .IsRequired();

            builder.Property(m => m.Sujet)
                .IsRequired();
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> f69b815974f0a4005de560cf3b0e2b257c691b83
