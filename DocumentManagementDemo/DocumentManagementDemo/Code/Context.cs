using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace DocumentManagementDemo
{
    public class DocumentsDb : DbContext
    {
        static DocumentsDb()
        {
            Database.SetInitializer<DocumentsDb>(null);
        }
        public DocumentsDb()
                : this("DocumentsConnectionString")
        {
        }
        public DocumentsDb(string connectionString)
                : base(connectionString)
        {
        }

        public DbSet<DocumentItem> Documents { get; set; }
        public DbSet<DocumentBinaryContentItem> DocumentBinaryContentItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DBDocumentItemMap());
            modelBuilder.Configurations.Add(new DBDocumentBinaryContentMap());
        }
    }

    public class DocumentItem
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public long ContentSize { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public virtual DocumentBinaryContentItem Content { get; set; }
        public virtual DocumentItem ParentItem { get; set; }

        public bool IsRoot { get { return ParentItem == null; } }
    }
    public class DocumentBinaryContentItem
    {
        public long ID { get; set; }
        public byte[] Data { get; set; }
    }

    public class DBDocumentItemMap : EntityTypeConfiguration<DocumentItem>
    {
        public DBDocumentItemMap()
        {
            HasKey(i => i.ID);
            Property(i => i.Name).IsRequired();
            Property(i => i.IsFolder).IsRequired();
            HasOptional(i => i.Content);
            HasOptional(i => i.ParentItem);
        }
    }
    public class DBDocumentBinaryContentMap : EntityTypeConfiguration<DocumentBinaryContentItem>
    {
        public DBDocumentBinaryContentMap()
        {
            HasKey(i => i.ID);
        }
    }
}