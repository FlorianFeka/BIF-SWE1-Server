USE [master]
GO

/****** Object:  Database [MonsterTradingCardGame]    Script Date: 22/01/2021 23:18:50 ******/
DROP DATABASE [MonsterTradingCardGame]
GO

/****** Object:  Database [MonsterTradingCardGame]    Script Date: 22/01/2021 23:18:50 ******/
CREATE DATABASE [MonsterTradingCardGame]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MonsterTradingCardGame', FILENAME = N'/var/opt/mssql/data/MonsterTradingCardGame.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MonsterTradingCardGame_log', FILENAME = N'/var/opt/mssql/data/MonsterTradingCardGame_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MonsterTradingCardGame].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [MonsterTradingCardGame] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET ARITHABORT OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [MonsterTradingCardGame] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [MonsterTradingCardGame] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET  DISABLE_BROKER 
GO

ALTER DATABASE [MonsterTradingCardGame] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [MonsterTradingCardGame] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET RECOVERY FULL 
GO

ALTER DATABASE [MonsterTradingCardGame] SET  MULTI_USER 
GO

ALTER DATABASE [MonsterTradingCardGame] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [MonsterTradingCardGame] SET DB_CHAINING OFF 
GO

ALTER DATABASE [MonsterTradingCardGame] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [MonsterTradingCardGame] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [MonsterTradingCardGame] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [MonsterTradingCardGame] SET QUERY_STORE = OFF
GO

ALTER DATABASE [MonsterTradingCardGame] SET  READ_WRITE 
GO


USE [MonsterTradingCardGame]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 22/01/2021 23:19:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 22/01/2021 23:19:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[Bio] [varchar](255) NULL,
	[Image] [varchar](255) NULL,
	[Money] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Users_Username] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [MonsterTradingCardGame]
GO

ALTER TABLE [dbo].[Sessions] DROP CONSTRAINT [FK_Sessions_Sessions_Users]
GO

/****** Object:  Table [dbo].[Sessions]    Script Date: 22/01/2021 23:19:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sessions]') AND type in (N'U'))
DROP TABLE [dbo].[Sessions]
GO

/****** Object:  Table [dbo].[Sessions]    Script Date: 22/01/2021 23:19:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sessions](
	[Id] [uniqueidentifier] NOT NULL,
	[Token] [varchar](255) NOT NULL,
	[ExpiryDateTime] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Sessions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Sessions_Users]
GO

USE [MonsterTradingCardGame]
GO

ALTER TABLE [dbo].[Cards] DROP CONSTRAINT [FK_Cards_Cards_User]
GO

/****** Object:  Table [dbo].[Cards]    Script Date: 22/01/2021 23:19:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cards]') AND type in (N'U'))
DROP TABLE [dbo].[Cards]
GO

/****** Object:  Table [dbo].[Cards]    Script Date: 22/01/2021 23:19:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Cards](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Damage] [float] NOT NULL,
	[Monster] [varchar](255) NULL,
	[Element] [varchar](255) NOT NULL,
	[IsSpell] [bit] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Cards] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Cards]  WITH CHECK ADD  CONSTRAINT [FK_Cards_Cards_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Cards] CHECK CONSTRAINT [FK_Cards_Cards_User]
GO

USE [MonsterTradingCardGame]
GO

ALTER TABLE [dbo].[Packages] DROP CONSTRAINT [FK_Packages_Packages_Cards]
GO

/****** Object:  Table [dbo].[Packages]    Script Date: 22/01/2021 23:20:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Packages]') AND type in (N'U'))
DROP TABLE [dbo].[Packages]
GO

/****** Object:  Table [dbo].[Packages]    Script Date: 22/01/2021 23:20:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Packages](
	[Id] [uniqueidentifier] NOT NULL,
	[CardId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Packages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[CardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Packages]  WITH CHECK ADD  CONSTRAINT [FK_Packages_Packages_Cards] FOREIGN KEY([CardId])
REFERENCES [dbo].[Cards] ([Id])
GO

ALTER TABLE [dbo].[Packages] CHECK CONSTRAINT [FK_Packages_Packages_Cards]
GO

