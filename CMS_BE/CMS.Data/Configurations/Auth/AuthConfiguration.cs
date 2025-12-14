using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Configurations.Auth
{
    public class AuthConfiguration : IEntityTypeConfiguration<User>
    {
        //public void Configure(EntityTypeBuilder<User> builder)
        //{
        //    builder.HasKey(x => x.Id);
        //}

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(s => s.Id);
        }
    }
}
