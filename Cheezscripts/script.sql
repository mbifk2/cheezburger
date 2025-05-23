USE [master]
GO
/****** Object:  Database [cheez]    Script Date: 11/26/2024 12:30:00 PM ******/
CREATE DATABASE [cheez]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'cheez', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\cheez.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'cheez_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\cheez_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [cheez] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [cheez].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [cheez] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [cheez] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [cheez] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [cheez] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [cheez] SET ARITHABORT OFF 
GO
ALTER DATABASE [cheez] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [cheez] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [cheez] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [cheez] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [cheez] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [cheez] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [cheez] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [cheez] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [cheez] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [cheez] SET  DISABLE_BROKER 
GO
ALTER DATABASE [cheez] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [cheez] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [cheez] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [cheez] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [cheez] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [cheez] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [cheez] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [cheez] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [cheez] SET  MULTI_USER 
GO
ALTER DATABASE [cheez] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [cheez] SET DB_CHAINING OFF 
GO
ALTER DATABASE [cheez] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [cheez] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [cheez] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [cheez] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [cheez] SET QUERY_STORE = ON
GO
ALTER DATABASE [cheez] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [cheez]
GO
/****** Object:  Table [dbo].[posts]    Script Date: 11/26/2024 12:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[posts](
	[PostID] [int] IDENTITY(1,1) NOT NULL,
	[FthreadID] [int] NULL,
	[Content] [text] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[CreatorId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[threads]    Script Date: 11/26/2024 12:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[threads](
	[FthreadID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NULL,
	[Title] [varchar](100) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsLocked] [bit] NULL,
	[CreatorId] [int] NOT NULL,
	[VerifiedOnly] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[FthreadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[topics]    Script Date: 11/26/2024 12:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[topics](
	[TopicID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[Description] [text] NULL,
	[CreatedAt] [datetime] NULL,
	[IsHidden] [bit] NULL,
	[CreatorId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 11/26/2024 12:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsBanned] [bit] NULL,
	[IsGuest] [bit] NULL,
	[IsOnline] [bit] NULL,
	[IsAdmin] [bit] NULL,
	[IsVerified] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[posts] ON 

INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (27, 20, N'Test post', CAST(N'2024-11-25T15:37:33.503' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (28, 20, N'Test post', CAST(N'2024-11-25T15:37:42.757' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (29, 20, N'Test post', CAST(N'2024-11-25T15:37:43.363' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (30, 20, N'Test post', CAST(N'2024-11-25T15:37:44.020' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (31, 20, N'Test post', CAST(N'2024-11-25T15:37:44.570' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (32, 20, N'Test post', CAST(N'2024-11-25T15:37:45.193' AS DateTime), 19)
INSERT [dbo].[posts] ([PostID], [FthreadID], [Content], [CreatedAt], [CreatorId]) VALUES (33, 20, N'Test post', CAST(N'2024-11-25T15:37:45.847' AS DateTime), 19)
SET IDENTITY_INSERT [dbo].[posts] OFF
GO
SET IDENTITY_INSERT [dbo].[threads] ON 

INSERT [dbo].[threads] ([FthreadID], [TopicID], [Title], [CreatedAt], [IsLocked], [CreatorId], [VerifiedOnly]) VALUES (20, 17, N'Test thread', CAST(N'2024-11-25T15:19:45.620' AS DateTime), 0, 19, 0)
SET IDENTITY_INSERT [dbo].[threads] OFF
GO
SET IDENTITY_INSERT [dbo].[topics] ON 

INSERT [dbo].[topics] ([TopicID], [Title], [Description], [CreatedAt], [IsHidden], [CreatorId]) VALUES (17, N'Test topic', N'Test description for topic', CAST(N'2024-11-25T13:19:18.240' AS DateTime), 0, 19)
SET IDENTITY_INSERT [dbo].[topics] OFF
GO
SET IDENTITY_INSERT [dbo].[users] ON 

INSERT [dbo].[users] ([UserID], [Username], [Email], [PasswordHash], [CreatedAt], [IsBanned], [IsGuest], [IsOnline], [IsAdmin], [IsVerified]) VALUES (18, N'user', N'test@test.com', N'AFwq2pFzdVM7s7Qdt/rbUw==:/rL8h7Hl3Mkj0SoU7MFq4voYhiLQ0fztWxgUUZiBmRc=', CAST(N'2024-11-25T15:17:41.540' AS DateTime), 0, 0, 1, 0, 0)
INSERT [dbo].[users] ([UserID], [Username], [Email], [PasswordHash], [CreatedAt], [IsBanned], [IsGuest], [IsOnline], [IsAdmin], [IsVerified]) VALUES (19, N'user2', N'test2@test.com', N'i0T8upGTrvzYCKepMnscog==:B3ZBXOf+6KNeAhDFUqwqILMxSs1nPTFi3X/n1qsuVno=', CAST(N'2024-11-25T15:17:45.840' AS DateTime), 0, 0, 1, 0, 0)
INSERT [dbo].[users] ([UserID], [Username], [Email], [PasswordHash], [CreatedAt], [IsBanned], [IsGuest], [IsOnline], [IsAdmin], [IsVerified]) VALUES (20, N'admin', N'test3@test.com', N'NwrHQDWnYkb4QZ4wFHB4jw==:upccfBwDVEnqt2hlPVs7BFspvknoUekNm5R5vf3UT6E=', CAST(N'2024-11-25T15:18:00.830' AS DateTime), 0, 0, 1, 0, 0)
SET IDENTITY_INSERT [dbo].[users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__users__536C85E4D11D6FAE]    Script Date: 11/26/2024 12:30:01 PM ******/
ALTER TABLE [dbo].[users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__users__A9D10534AF6FB3E2]    Script Date: 11/26/2024 12:30:01 PM ******/
ALTER TABLE [dbo].[users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[posts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[threads] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[threads] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[threads] ADD  DEFAULT ((0)) FOR [VerifiedOnly]
GO
ALTER TABLE [dbo].[topics] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[topics] ADD  DEFAULT ((0)) FOR [IsHidden]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0)) FOR [IsBanned]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0)) FOR [IsGuest]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0)) FOR [IsOnline]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[posts]  WITH CHECK ADD FOREIGN KEY([CreatorId])
REFERENCES [dbo].[users] ([UserID])
GO
ALTER TABLE [dbo].[posts]  WITH CHECK ADD FOREIGN KEY([FthreadID])
REFERENCES [dbo].[threads] ([FthreadID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[threads]  WITH CHECK ADD FOREIGN KEY([TopicID])
REFERENCES [dbo].[topics] ([TopicID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[threads]  WITH CHECK ADD FOREIGN KEY([CreatorId])
REFERENCES [dbo].[users] ([UserID])
GO
ALTER TABLE [dbo].[topics]  WITH CHECK ADD FOREIGN KEY([CreatorId])
REFERENCES [dbo].[users] ([UserID])
GO
USE [master]
GO
ALTER DATABASE [cheez] SET  READ_WRITE 
GO
