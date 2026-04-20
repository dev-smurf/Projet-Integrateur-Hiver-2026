IF OBJECT_ID(N'dbo.Equipes', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Equipes](
        [Id] uniqueidentifier NOT NULL,
        [NameFr] nvarchar(200) NOT NULL,
        [NameEn] nvarchar(200) NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Equipes] PRIMARY KEY ([Id])
    );
END
GO

IF OBJECT_ID(N'dbo.Conversations', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Conversations](
        [Id] uniqueidentifier NOT NULL,
        [AdminId] uniqueidentifier NOT NULL,
        [MemberId] uniqueidentifier NOT NULL,
        [LastMessageAt] datetime2 NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Conversations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Conversations_AspNetUsers_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Conversations_AspNetUsers_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    CREATE INDEX [IX_Conversations_AdminId] ON [dbo].[Conversations] ([AdminId]);
    CREATE INDEX [IX_Conversations_MemberId] ON [dbo].[Conversations] ([MemberId]);
END
GO

IF OBJECT_ID(N'dbo.Messages', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Messages](
        [Id] uniqueidentifier NOT NULL,
        [Texte] nvarchar(max) NULL,
        [ExpediteurId] uniqueidentifier NOT NULL,
        [ReceveurId] uniqueidentifier NOT NULL,
        [Date] datetime2 NOT NULL,
        [ConversationId] uniqueidentifier NOT NULL,
        [ReadAt] datetime2 NULL,
        [AttachmentUrl] nvarchar(max) NULL,
        [AttachmentFileName] nvarchar(max) NULL,
        [AttachmentContentType] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_AspNetUsers_ExpediteurId] FOREIGN KEY ([ExpediteurId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Messages_AspNetUsers_ReceveurId] FOREIGN KEY ([ReceveurId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Messages_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Messages_ConversationId] ON [dbo].[Messages] ([ConversationId]);
    CREATE INDEX [IX_Messages_ExpediteurId] ON [dbo].[Messages] ([ExpediteurId]);
    CREATE INDEX [IX_Messages_ReceveurId] ON [dbo].[Messages] ([ReceveurId]);
END
GO

IF OBJECT_ID(N'dbo.Progression', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Progression](
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Niveau] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Progression] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Progression_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    CREATE INDEX [IX_Progression_UserId] ON [dbo].[Progression] ([UserId]);
END
GO

IF OBJECT_ID(N'dbo.Rdv', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Rdv](
        [Id] uniqueidentifier NOT NULL,
        [Titre] nvarchar(200) NOT NULL,
        [Date] datetime2 NOT NULL,
        [Duree] time NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Note] nvarchar(500) NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Rdv] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rdv_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    CREATE INDEX [IX_Rdv_UserId] ON [dbo].[Rdv] ([UserId]);
END
GO

IF OBJECT_ID(N'dbo.EquipeMembres', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[EquipeMembres](
        [EquipeId] uniqueidentifier NOT NULL,
        [MembresId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_EquipeMembres] PRIMARY KEY ([EquipeId], [MembresId]),
        CONSTRAINT [FK_EquipeMembres_AspNetUsers_MembresId] FOREIGN KEY ([MembresId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EquipeMembres_Equipes_EquipeId] FOREIGN KEY ([EquipeId]) REFERENCES [dbo].[Equipes] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_EquipeMembres_MembresId] ON [dbo].[EquipeMembres] ([MembresId]);
END
GO

IF OBJECT_ID(N'dbo.Archives', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Archives](
        [Id] uniqueidentifier NOT NULL,
        [ProgressionId] uniqueidentifier NULL,
        [RdvId] uniqueidentifier NULL,
        [EquipeId] uniqueidentifier NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Archives] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Archives_Equipes_EquipeId] FOREIGN KEY ([EquipeId]) REFERENCES [dbo].[Equipes] ([Id]),
        CONSTRAINT [FK_Archives_Progression_ProgressionId] FOREIGN KEY ([ProgressionId]) REFERENCES [dbo].[Progression] ([Id]),
        CONSTRAINT [FK_Archives_Rdv_RdvId] FOREIGN KEY ([RdvId]) REFERENCES [dbo].[Rdv] ([Id])
    );

    CREATE INDEX [IX_Archives_EquipeId] ON [dbo].[Archives] ([EquipeId]);
    CREATE INDEX [IX_Archives_ProgressionId] ON [dbo].[Archives] ([ProgressionId]);
    CREATE INDEX [IX_Archives_RdvId] ON [dbo].[Archives] ([RdvId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260319003658_CleanInit')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319003658_CleanInit', N'10.0.2');
END
GO
