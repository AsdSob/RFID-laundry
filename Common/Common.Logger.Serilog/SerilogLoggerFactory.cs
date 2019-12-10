using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logger.Serilog
{
    public class SerilogLoggerFactory : ILoggerFactory
    {
        private readonly global::Serilog.Core.Logger _baseLogger;

        private readonly Dictionary<string, object> _enricherProperties;
        
        public SerilogLoggerFactory()
        {
            var serilogConfigurator = CreateLogConfugurator();

            //_enricherProperties = enricherProperties;

            var configuration = serilogConfigurator.Configuration ?? new LoggerConfiguration();

            if (!string.IsNullOrWhiteSpace(serilogConfigurator.ConfigPath))
            {
                configuration
                    .ReadFrom.AppSettings(null, serilogConfigurator.ConfigPath);
            }

            serilogConfigurator.ConfigureSerilog(configuration);
            configuration.MinimumLevel.Verbose();

            _baseLogger = configuration.CreateLogger();
        }

        /// <summary>
        /// Создает логгер
        /// </summary>
        /// <param name="loggerName">Имя логера</param>
        /// <param name="enricherProperties">Дополнительные параметры для энричера</param>
        /// <param name="correlationId">Id который будет содержаться во всех сообщениях данного логера (для структурного логирования)</param>
        /// <returns>Логер</returns>
        public ILogger CreateLogger(string loggerName, string correlationId = null)
        {
            if (string.IsNullOrEmpty(loggerName)) throw new ArgumentNullException(nameof(loggerName));

            var enriches = new List<ILogEventEnricher>
            {
                new PropertyEnricher("Name", loggerName)
            };

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                enriches.Add(new PropertyEnricher("CorrelationId", correlationId));
            }

            if (_enricherProperties != null)
            {
                foreach (var property in _enricherProperties)
                {
                    enriches.Add(new PropertyEnricher(property.Key, property.Value));
                }
            }

            var loggerWrapper = _baseLogger.ForContext(enriches);
            return new SerilogLogger(loggerName, loggerWrapper);
        }

        private ISerilogConfigurator CreateLogConfugurator()
        {
            // конфигуратор серилога
            var loggerConfig = new LoggerConfiguration()
                //.ReadFrom.Configuration(Configuration) TODO
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                    "{NewLine}{Exception}")
                .WriteTo.RollingFile(pathFormat: @"logs\host-{Date}.log",
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                    "{NewLine}{Exception}",
                    fileSizeLimitBytes: 1_000_000,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1));

            return new SerilogerConfigurator(loggerConfig);
        }
    }

    public class SerilogerConfigurator : ISerilogConfigurator
    {
        private readonly List<ElasticsearchSinkOptions> _elasticsearchOptions;
        private readonly List<FileSinkOptions> _fileSinkOptions;

        private bool _isConsoneEnabled;
        private LogEventLevel _consoleMinLevel;

        public LoggerConfiguration Configuration { get; }

        public SerilogerConfigurator()
        {
            _elasticsearchOptions = new List<ElasticsearchSinkOptions>();
            _fileSinkOptions = new List<FileSinkOptions>();

            _isConsoneEnabled = false;
            _consoleMinLevel = LogEventLevel.Fatal;
        }

        public SerilogerConfigurator(LoggerConfiguration cfg)
        {
            _elasticsearchOptions = new List<ElasticsearchSinkOptions>();
            _fileSinkOptions = new List<FileSinkOptions>();

            _isConsoneEnabled = false;
            _consoleMinLevel = LogEventLevel.Fatal;

            Configuration = cfg;
        }

        public ISerilogConfigurator ConfigureSerilog(LoggerConfiguration configuration)
        {
            foreach (var elasticsearchOption in _elasticsearchOptions ?? new List<ElasticsearchSinkOptions>(0))
            {
                configuration.WriteTo.Async(conf => conf.Elasticsearch(elasticsearchOption));
            }

            foreach (var fileSinkOption in _fileSinkOptions ?? new List<FileSinkOptions>(0))
            {
                var isShared = fileSinkOption.IsShared;
                if (fileSinkOption.IsAsync)
                {
                    configuration
                        .WriteTo
                        .Async(conf =>
                        {
                            conf.File(fileSinkOption.Path,
                                fileSinkOption.MinLevel,
                                rollingInterval: fileSinkOption.Interval,
                                fileSizeLimitBytes: fileSinkOption.FileSizeLimitBytes,
                                rollOnFileSizeLimit: true,
                                shared: isShared);
                        });
                }
                else
                {
                    configuration
                        .WriteTo
                        .File(
                            fileSinkOption.Path,
                            fileSinkOption.MinLevel,
                            rollingInterval: fileSinkOption.Interval,
                            fileSizeLimitBytes: fileSinkOption.FileSizeLimitBytes,
                            rollOnFileSizeLimit: true,
                            shared: isShared);
                }
            }

            if (_isConsoneEnabled)
            {
                configuration
                    .WriteTo
                    .Console(_consoleMinLevel);
            }

            return this;
        }

        /// <summary>
        /// Path of configuration file.
        /// </summary>
        public string ConfigPath { get; set; }



        /// <summary>
        /// Добавить настройку для записи в Elasticsearch
        /// </summary>
        /// <param name="elasticEndpoint"> Адрес Elasticsearch</param>
        /// <param name="minLevel"> минимальный уровень </param>
        /// <param name="indexBaseName">База имени индекса. 
        /// Дефолтное имя: "logstash"
        /// </param>
        public SerilogerConfigurator AddElasticSearchTarget(Uri elasticEndpoint, LogEventLevel minLevel,
            string indexBaseName = null)
        {
            var option = new ElasticsearchSinkOptions(elasticEndpoint);
            option.MinimumLogEventLevel = minLevel;

            if (string.IsNullOrEmpty(indexBaseName))
            {
                option.IndexFormat = Assembly.GetEntryAssembly().GetName().Name.ToLower().Replace('.', '_');
            }
            else
            {
                option.IndexFormat = indexBaseName;
            }

            _elasticsearchOptions.Add(option);

            return this;
        }

        /// <summary>
        /// Добавить таргет для записи в файл
        /// </summary>
        /// <param name="path">Относительный или абсолютный путь к файлу (дата к имени файла будет добавлена автоматически)</param>
        /// <param name="minLevel">Мин. уровень</param>
        /// <param name="interval">Интервал создания новых файлов </param>
        /// <param name="fileSizeLimitMBytes">Лимит размера каждого файла с логами (MB). 1 GB по дефолту.</param>
        /// <param name="isShared">Конкурентный доступ к файлу.</param>
        /// <param name="isAsync">Асинхронная запись.</param>
        public SerilogerConfigurator AddFileTarget(
            string path,
            LogEventLevel minLevel,
            RollingInterval interval = RollingInterval.Day,
            long? fileSizeLimitMBytes = 1024,
            bool isShared = true,
            bool isAsync = false)
        {
            if (string.IsNullOrWhiteSpace(path)) return this;

            _fileSinkOptions.Add(new FileSinkOptions
            {
                Path = path,
                MinLevel = minLevel,
                Interval = interval,
                FileSizeLimitBytes = fileSizeLimitMBytes.MegabytesToBytes(),
                IsAsync = isAsync,
                IsShared = isShared
            });

            return this;
        }

        /// <summary>
        /// Активировать запись в консоль
        /// </summary>
        /// <param name="minLevel"></param>
        public SerilogerConfigurator EnableConsoleOutput(LogEventLevel minLevel)
        {
            _isConsoneEnabled = true;
            _consoleMinLevel = minLevel;

            return this;
        }

        private class FileSinkOptions
        {
            public string Path { get; set; }
            public LogEventLevel MinLevel { get; set; }

            /// <summary>
            /// Интервал создания новых файлов с логами
            /// </summary>
            public RollingInterval Interval { get; set; }

            /// <summary>
            /// Лимит размера каждого файла с логами (MB).
            /// Если null, то размер файлов неограничен.
            /// При превышении лимита создается новый файл.
            /// </summary>
            public long? FileSizeLimitBytes { get; set; }

            /// <summary>
            /// Включает конкурентную запись в файл. 
            /// </summary>
            public bool IsShared { get; set; }

            /// <summary>
            /// Включает асинхронную запись в файл
            /// </summary>
            public bool IsAsync { get; set; }
        }
    }

    public static class SerilogUtils
    {
        /// <summary>
        /// Convert from megabytes to bytes
        /// </summary>
        /// <param name="mbytes"></param>
        /// <returns></returns>
        public static long? MegabytesToBytes(this long? mbytes)
        {
            return mbytes * 1024 * 1024;
        }
    }

    /// <summary>
    /// Used SeriloggerFactory by Fluent API. 
    /// For more info about LoggerConfiguration see: 
    /// https://github.com/serilog/serilog/wiki/Configuration-Basics
    /// </summary>
    public interface ISerilogConfigurator
    {
        ISerilogConfigurator ConfigureSerilog(LoggerConfiguration configuration);

        string ConfigPath { get; }

        LoggerConfiguration Configuration { get; }
    }
}
