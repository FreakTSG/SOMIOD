CREATE TABLE [dbo].[Data] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
	[Name] NVARCHAR (50) NOT NULL,
    [Content]     NVARCHAR (500) NOT NULL,
    [Creation_dt] NVARCHAR (50)  NOT NULL,
    [Parent]      INT            NOT NULL,
  
);
