CREATE TABLE [dbo].[CatchUpSubscription](
	[CatchUpSubscriptionId] [uniqueidentifier] NOT NULL,
	[CreateDateTime] [datetime2](7) NOT NULL,
	[EndpointId] [uniqueidentifier] NOT NULL,
	[EventStoreServerId] [uniqueidentifier] NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[LastCheckpoint] [int] NULL,
	[StreamName] [nvarchar](200) NOT NULL,
	[EndPointUri] [varchar](max) NOT NULL,
 CONSTRAINT [PK_CatchUpSubscription] PRIMARY KEY CLUSTERED 
(
	[CatchUpSubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
