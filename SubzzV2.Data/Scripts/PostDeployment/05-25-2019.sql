ALTER TABLE [Subzz_Users].[Users].[Users]
ADD IsViewedNewVersion BIT DEFAULT(0) NOT NULL

----
USE [Subzz_Settings]
GO
CREATE TABLE [Settings].[Versions](
	[Id] [int] NOT NULL,
	[Version] [varchar](20) NULL,
	[Features] [varchar](500) NULL,
	[ContentDetails] [varchar](1500) NULL,
 CONSTRAINT [PK_Versions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO