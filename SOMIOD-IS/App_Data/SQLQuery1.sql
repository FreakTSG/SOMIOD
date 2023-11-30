CREATE TABLE [dbo].[Data] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
	[Name] NVARCHAR (50) NOT NULL,
    [Content]     NVARCHAR (500) NOT NULL,
    [Creation_dt] NVARCHAR (50)  NOT NULL,
    [Parent]      INT            NOT NULL,
  
);
CREATE TABLE [dbo].[Container]
(
	[Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Creation_dt] NVARCHAR (50) NOT NULL,
    [Parent]      INT           NOT NULL,
);
CREATE TABLE [dbo].[Subscription]
(
	[Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Creation_dt] NVARCHAR (50) NOT NULL,
    [Parent]      INT           NOT NULL,
    [Event]       NVARCHAR (50) NOT NULL,
    [Endpoint]    NVARCHAR (50) NOT NULL,
);
CREATE TABLE [dbo].[Application]
(
	[Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [Creation_dt] NVARCHAR(50) NOT NULL
)

