using System;
using Directory.Abstractions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Directory {
    public class DefaultPictureProvider : IDefaultPictureProvider {
        private readonly Lazy<byte[]> _cachedPicture;
        private readonly IConfiguration _configuration;

        public DefaultPictureProvider(IConfiguration configuration) {
            _configuration = configuration;
            _cachedPicture = new Lazy<byte[]>(LoadDefaultFile);
        }

        /// <inheritdoc cref="IDefaultPictureProvider.GetDefaultPicture()"/>
        public byte[] GetDefaultPicture() {
            return _cachedPicture.Value;
        }

        private byte[] LoadDefaultFile() {
            string defaultLocation = _configuration.GetValue<string>(Constants.Config.DefaultPictureFileKey);

            if (!string.IsNullOrEmpty(defaultLocation) && File.Exists(defaultLocation)) {
                return File.ReadAllBytes(defaultLocation);
            }

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.DefaultBuiltInPictureLocation)
                    ?? throw new IOException("Failed to get built-in default picture.");

            // If the stream is null, this should probably just throw an exception.
            // But it shouldn't be, especially since this is covered by unit tests.
            byte[] contents = new byte[stream.Length];
            stream.Read(contents, 0, contents.Length);

            return contents;
        }
    }
}
