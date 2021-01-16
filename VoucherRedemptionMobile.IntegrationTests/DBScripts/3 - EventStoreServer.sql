CREATE TABLE [dbo].[EventStoreServer](
	[EventStoreServerId] [uniqueidentifier] NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EventStoreServer] PRIMARY KEY CLUSTERED 
(
	[EventStoreServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

