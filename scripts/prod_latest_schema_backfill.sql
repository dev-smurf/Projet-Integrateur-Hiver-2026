IF COL_LENGTH('dbo.Modules', 'Name') IS NULL
BEGIN
    ALTER TABLE [dbo].[Modules] ADD [Name] nvarchar(100) NULL;
END
GO

IF COL_LENGTH('dbo.Modules', 'Subject') IS NULL
BEGIN
    ALTER TABLE [dbo].[Modules] ADD [Subject] nvarchar(200) NULL;
END
GO

IF COL_LENGTH('dbo.Modules', 'Content') IS NULL
BEGIN
    ALTER TABLE [dbo].[Modules] ADD [Content] nvarchar(max) NULL;
END
GO

IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Modules'
      AND COLUMN_NAME = 'NameFr'
      AND IS_NULLABLE = 'NO'
)
BEGIN
    ALTER TABLE [dbo].[Modules] ALTER COLUMN [NameFr] nvarchar(100) NULL;
END
GO

IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Modules'
      AND COLUMN_NAME = 'NameEn'
      AND IS_NULLABLE = 'NO'
)
BEGIN
    ALTER TABLE [dbo].[Modules] ALTER COLUMN [NameEn] nvarchar(100) NULL;
END
GO

UPDATE [dbo].[Modules]
SET
    [Name] = COALESCE([Name], [NameFr]),
    [Subject] = COALESCE([Subject], [SujetFr]),
    [Content] = COALESCE([Content], [ContenueFr]);
GO

IF OBJECT_ID(N'dbo.Quizz', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Quizz](
        [Id] uniqueidentifier NOT NULL,
        [Titre] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Quizz] PRIMARY KEY ([Id])
    );
END
GO

IF OBJECT_ID(N'dbo.QuizAssignments', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizAssignments](
        [Id] uniqueidentifier NOT NULL,
        [QuizId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AssignedAt] datetime2 NOT NULL,
        [DueDate] datetime2 NULL,
        [CompletedAt] datetime2 NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_QuizAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuizAssignments_Quizz_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [dbo].[Quizz] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QuizAssignments_QuizId] ON [dbo].[QuizAssignments] ([QuizId]);
END
GO

IF OBJECT_ID(N'dbo.QuizQuestions', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizQuestions](
        [Id] uniqueidentifier NOT NULL,
        [QuizId] uniqueidentifier NOT NULL,
        [QuestionText] nvarchar(max) NOT NULL,
        [Order] int NOT NULL,
        [QuestionType] int NOT NULL,
        [Placeholder] nvarchar(max) NULL,
        [ScaleMinLabel] nvarchar(max) NOT NULL CONSTRAINT [DF_QuizQuestions_ScaleMinLabel] DEFAULT N'Jamais',
        [ScaleMidLabel] nvarchar(max) NOT NULL CONSTRAINT [DF_QuizQuestions_ScaleMidLabel] DEFAULT N'Parfois',
        [ScaleMaxLabel] nvarchar(max) NOT NULL CONSTRAINT [DF_QuizQuestions_ScaleMaxLabel] DEFAULT N'Toujours',
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_QuizQuestions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuizQuestions_Quizz_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [dbo].[Quizz] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QuizQuestions_QuizId] ON [dbo].[QuizQuestions] ([QuizId]);
END
GO

IF OBJECT_ID(N'dbo.QuizQuestionResponses', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizQuestionResponses](
        [Id] uniqueidentifier NOT NULL,
        [QuizQuestionId] uniqueidentifier NOT NULL,
        [ResponseText] nvarchar(max) NOT NULL,
        [Order] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_QuizQuestionResponses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuizQuestionResponses_QuizQuestions_QuizQuestionId] FOREIGN KEY ([QuizQuestionId]) REFERENCES [dbo].[QuizQuestions] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QuizQuestionResponses_QuizQuestionId] ON [dbo].[QuizQuestionResponses] ([QuizQuestionId]);
END
GO

IF OBJECT_ID(N'dbo.UserQuizResponses', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserQuizResponses](
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [QuizQuestionId] uniqueidentifier NOT NULL,
        [SelectedScore] int NULL,
        [SelectedResponseId] uniqueidentifier NULL,
        [SelectedTextResponse] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_UserQuizResponses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserQuizResponses_QuizQuestions_QuizQuestionId] FOREIGN KEY ([QuizQuestionId]) REFERENCES [dbo].[QuizQuestions] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserQuizResponses_QuizQuestionId] ON [dbo].[UserQuizResponses] ([QuizQuestionId]);
END
GO

IF OBJECT_ID(N'dbo.AdminAvailabilities', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AdminAvailabilities](
        [Id] uniqueidentifier NOT NULL,
        [AdminId] uniqueidentifier NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_AdminAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdminAvailabilities_AspNetUsers_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    CREATE INDEX [IX_AdminAvailabilities_AdminId] ON [dbo].[AdminAvailabilities] ([AdminId]);
END
GO

IF OBJECT_ID(N'dbo.AdminAvailabilityOverrides', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AdminAvailabilityOverrides](
        [Id] uniqueidentifier NOT NULL,
        [AdminId] uniqueidentifier NOT NULL,
        [Date] datetime2 NOT NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [IsBlocked] bit NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_AdminAvailabilityOverrides] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdminAvailabilityOverrides_AspNetUsers_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    CREATE INDEX [IX_AdminAvailabilityOverrides_AdminId] ON [dbo].[AdminAvailabilityOverrides] ([AdminId]);
END
GO

IF OBJECT_ID(N'dbo.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Appointments](
        [Id] uniqueidentifier NOT NULL,
        [MemberId] uniqueidentifier NOT NULL,
        [AdminId] uniqueidentifier NOT NULL,
        [Date] datetime2 NOT NULL,
        [Duration] time NOT NULL,
        [Motif] nvarchar(500) NULL,
        [Status] int NOT NULL,
        [RefusalReason] nvarchar(500) NULL,
        [ConversationId] uniqueidentifier NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Appointments_AspNetUsers_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Appointments_AspNetUsers_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Appointments_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([Id])
    );

    CREATE INDEX [IX_Appointments_AdminId] ON [dbo].[Appointments] ([AdminId]);
    CREATE INDEX [IX_Appointments_ConversationId] ON [dbo].[Appointments] ([ConversationId]);
    CREATE INDEX [IX_Appointments_MemberId] ON [dbo].[Appointments] ([MemberId]);
END
GO

IF COL_LENGTH('dbo.Messages', 'AppointmentId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Messages] ADD [AppointmentId] uniqueidentifier NULL;
END
GO

IF COL_LENGTH('dbo.Messages', 'Type') IS NULL
BEGIN
    ALTER TABLE [dbo].[Messages] ADD [Type] int NOT NULL CONSTRAINT [DF_Messages_Type] DEFAULT (0);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Messages_Appointments_AppointmentId'
)
BEGIN
    ALTER TABLE [dbo].[Messages]
    ADD CONSTRAINT [FK_Messages_Appointments_AppointmentId]
        FOREIGN KEY ([AppointmentId]) REFERENCES [dbo].[Appointments] ([Id]) ON DELETE SET NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Messages_AppointmentId'
      AND object_id = OBJECT_ID(N'dbo.Messages')
)
BEGIN
    CREATE INDEX [IX_Messages_AppointmentId] ON [dbo].[Messages] ([AppointmentId]);
END
GO

IF OBJECT_ID(N'dbo.ModuleSections', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ModuleSections](
        [Id] uniqueidentifier NOT NULL,
        [ModuleId] uniqueidentifier NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Content] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ModuleSections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ModuleSections_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ModuleSections_ModuleId_SortOrder] ON [dbo].[ModuleSections] ([ModuleId], [SortOrder]);
END
GO

IF OBJECT_ID(N'dbo.MemberModules', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MemberModules](
        [Id] uniqueidentifier NOT NULL,
        [MemberId] uniqueidentifier NOT NULL,
        [ModuleId] uniqueidentifier NOT NULL,
        [ProgressPercent] int NOT NULL CONSTRAINT [DF_MemberModules_ProgressPercent] DEFAULT (0),
        [IsCompleted] bit NOT NULL CONSTRAINT [DF_MemberModules_IsCompleted] DEFAULT (0),
        [Created] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModified] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [Deleted] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_MemberModules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MemberModules_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members] ([Id]),
        CONSTRAINT [FK_MemberModules_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules] ([Id])
    );

    CREATE INDEX [IX_MemberModules_ModuleId] ON [dbo].[MemberModules] ([ModuleId]);
    CREATE UNIQUE INDEX [IX_MemberModules_MemberId_ModuleId] ON [dbo].[MemberModules] ([MemberId], [ModuleId]) WHERE [Deleted] IS NULL;
END
GO
