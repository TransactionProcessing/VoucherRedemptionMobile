namespace VoucherRedemptionMobile.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SQLite;
    using VoucherRedemption.Clients;
    using LogMessage = Entities.LogMessage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IDatabaseContext" />
    public class DatabaseContext : IDatabaseContext
    {
        #region Fields

        /// <summary>
        /// The connection
        /// </summary>
        private readonly SQLiteAsyncConnection Connection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        /// <param name="connectionStringResolver">The connection string resolver.</param>
        public DatabaseContext(Func<String> connectionStringResolver) : this(connectionStringResolver())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DatabaseContext(String connectionString)
        {
            this.Connection = new SQLiteAsyncConnection(connectionString);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the debug log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateDebugLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Debug);
        }

        /// <summary>
        /// Creates the error log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateErrorLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Error);
        }

        /// <summary>
        /// Creates the error log messages.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static List<LogMessage> CreateErrorLogMessages(Exception exception)
        {
            List<LogMessage> logMessages = new List<LogMessage>();

            Exception e = exception;
            while (e != null)
            {
                logMessages.Add(DatabaseContext.CreateLogMessage(e.Message, LogLevel.Error));
                e = e.InnerException;
            }

            return logMessages;
        }

        /// <summary>
        /// Creates the fatal log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateFatalLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Fatal);
        }

        /// <summary>
        /// Creates the fatal log messages.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static List<LogMessage> CreateFatalLogMessages(Exception exception)
        {
            List<LogMessage> logMessages = new List<LogMessage>();

            Exception e = exception;
            while (e != null)
            {
                logMessages.Add(DatabaseContext.CreateLogMessage(e.Message, LogLevel.Fatal));
                e = e.InnerException;
            }

            return logMessages;
        }

        /// <summary>
        /// Creates the information log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateInformationLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Creates the trace log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateTraceLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Trace);
        }

        /// <summary>
        /// Creates the warning log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static LogMessage CreateWarningLogMessage(String message)
        {
            return DatabaseContext.CreateLogMessage(message, LogLevel.Warn);
        }

        /// <summary>
        /// Gets the log messages.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        public async Task<List<LogMessage>> GetLogMessages(Int32 batchSize)
        {
            List<LogMessage> messages = await this.Connection.Table<LogMessage>().OrderBy(l => l.EntryDateTime).Take(batchSize).ToListAsync();

            return messages;
        }

        /// <summary>
        /// Initialises the database.
        /// </summary>
        public async Task InitialiseDatabase()
        {
            // Create the required tables
            await this.Connection.CreateTableAsync<LogMessage>();
        }

        /// <summary>
        /// Inserts the log message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public async Task InsertLogMessage(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);

            LogLevel messageLevel = (LogLevel)Enum.Parse(typeof(LogLevel), logMessage.LogLevel, true);
            if (App.Configuration == null || messageLevel <= App.Configuration.LogLevel)
            {
                await this.Connection.InsertAsync(logMessage);
            }
        }

        /// <summary>
        /// Inserts the log messages.
        /// </summary>
        /// <param name="logMessages">The log messages.</param>
        public async Task InsertLogMessages(List<LogMessage> logMessages)
        {
            foreach (LogMessage logMessage in logMessages)
            {
                await this.InsertLogMessage(logMessage);
            }
        }

        /// <summary>
        /// Removes the uploaded messages.
        /// </summary>
        /// <param name="logMessagesToRemove">The log messages to remove.</param>
        public async Task RemoveUploadedMessages(List<LogMessage> logMessagesToRemove)
        {
            foreach (LogMessage logMessage in logMessagesToRemove)
            {
                await this.Connection.DeleteAsync(logMessage);
            }
        }
        
        /// <summary>
        /// Creates the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        private static LogMessage CreateLogMessage(String message,
                                                   LogLevel logLevel)
        {
            return new LogMessage
                   {
                       EntryDateTime = DateTime.UtcNow,
                       Message = message,
                       LogLevel = logLevel.ToString()
                   };
        }

        #endregion
    }
}