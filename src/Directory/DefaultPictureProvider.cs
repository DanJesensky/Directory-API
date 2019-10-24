using Directory.Abstractions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Directory {
    public class DefaultPictureProvider : IDefaultPictureProvider {
        private byte[] _cachedPicture;
        private readonly IConfiguration _configuration;

        public DefaultPictureProvider(IConfiguration configuration) {
            _configuration = configuration;
            UpdateCachedFile();
        }

        /// <inheritdoc cref="IDefaultPictureProvider.GetDefaultPicture()"/>
        public byte[] GetDefaultPicture() {
            return _cachedPicture;
        }

        private void UpdateCachedFile() {
            string defaultLocation = _configuration.GetValue<string>(Constants.Config.DefaultPictureFileKey);

            if (string.IsNullOrEmpty(defaultLocation) || !File.Exists(defaultLocation)) {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.DefaultBuiltInPictureLocation)) {
                    _cachedPicture = new byte[stream.Length];
                    stream.Read(_cachedPicture, 0, _cachedPicture.Length);
                }
            } else {
                _cachedPicture = File.ReadAllBytes(defaultLocation);
            }
        }
    }
}
