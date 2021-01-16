CREATE TABLE [dbo].[Subscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[EventStoreId] [uniqueidentifier] NOT NULL,
	[StreamName] [varchar](max) NOT NULL,
	[GroupName] [varchar](max) NOT NULL,
	[EndPointUri] [varchar](max) NOT NULL,
	[StreamPosition] [int] NULL,
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC,
	[EventStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
