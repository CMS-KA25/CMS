// using CMS.Domain.Auth.Entities;
// using CMS.Domain.Notifications.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace CMS.Data.Configurations.Notifications
// {
//     public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
//     {
//         public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
//         {
//             // Primary Key
//             builder.HasKey(nt => nt.TemplateID);

//             // Properties
//             builder.Property(nt => nt.TemplateName)
//                 .IsRequired()
//                 .HasMaxLength(255);

//             builder.Property(nt => nt.Subject)
//                 .IsRequired()
//                 .HasMaxLength(255);

//             builder.Property(nt => nt.MessageBody)
//                 .IsRequired()
//                 .HasMaxLength(2000);

//             builder.Property(nt => nt.TriggerEvent)
//                 .IsRequired()
//                 .HasMaxLength(255);

//             builder.Property(nt => nt.IsActive)
//                 .HasDefaultValue(true);

//             builder.Property(nt => nt.CreatedAt)
//                 .HasDefaultValueSql("GETUTCDATE()");

//             builder.Property(nt => nt.UpdatedAt)
//                 .HasDefaultValueSql("GETUTCDATE()");

//             builder.Property(nt => nt.IsDeleted)
//                 .HasDefaultValue(false);

//             // Soft Delete Query Filter
//             builder.HasQueryFilter(nt => !nt.IsDeleted);
//         }
//     }

//     public class NotificationInstanceConfiguration : IEntityTypeConfiguration<NotificationInstance>
//     {
//         public void Configure(EntityTypeBuilder<NotificationInstance> builder)
//         {
//             // Primary Key
//             builder.HasKey(ni => ni.NotificationID);

//             // Foreign Keys
//             builder.HasOne<NotificationTemplate>()
//                 .WithMany()
//                 .HasForeignKey(ni => ni.TemplateID)
//                 .OnDelete(DeleteBehavior.NoAction);

//             builder.HasOne<User>() // CreatedBy user
//                 .WithMany()
//                 .HasForeignKey(ni => ni.CreatedBy)
//                 .OnDelete(DeleteBehavior.NoAction);

//             // Properties
//             builder.Property(ni => ni.RecipientIDs)
//                 .IsRequired()
//                 .HasMaxLength(2000); // JSON array of GUIDs

//             builder.Property(ni => ni.Subject)
//                 .HasMaxLength(255);

//             builder.Property(ni => ni.Message)
//                 .HasMaxLength(2000);

//             builder.Property(ni => ni.RecipientContact)
//                 .HasMaxLength(1000); // JSON array of contacts

//             builder.Property(ni => ni.CreatedAt)
//                 .HasDefaultValueSql("GETUTCDATE()");

//             builder.Property(ni => ni.UpdatedAt)
//                 .HasDefaultValueSql("GETUTCDATE()");

//             builder.Property(ni => ni.IsDeleted)
//                 .HasDefaultValue(false);

//             // Soft Delete Query Filter
//             builder.HasQueryFilter(ni => !ni.IsDeleted);
//         }
//     }
// }