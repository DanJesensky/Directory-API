using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Directory.Test {
    [TestFixture]
    public class DefaultPictureProviderTest {
        /// <summary>
        /// Ensures that the file
        /// </summary>
        [Test]
        public void NoConfiguredLocation_UsesDefault() {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            byte[] expected;
            using (Stream stream = typeof(DefaultPictureProvider).Assembly.GetManifestResourceStream(Constants.DefaultBuiltInPictureLocation)) {
                expected = new byte[stream.Length];
                stream.Read(expected, 0, expected.Length);
            }

            DefaultPictureProvider provider = new DefaultPictureProvider(configuration);
            byte[] actual = provider.GetDefaultPicture();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ConfiguredLocation_UsesConfiguredFile() {
            // Despite the method name, GetTempFileName actually returns an absolute path (not just the name).
            string tempFilePath = Path.GetTempFileName();

            try {
                byte[] expectedFile = new byte[100];

                // Fill the byte array with something, don't really care
                // what (as long as we know what it has in it and it isn't zero-length)
                for (byte i = 0; i < expectedFile.Length; i++) {
                    expectedFile[i] = i;
                }

                using (Stream stream = new FileStream(tempFilePath, FileMode.Open)) {
                    stream.Write(expectedFile, 0, expectedFile.Length);
                }

                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new[] {
                    new KeyValuePair<string, string>(Constants.Config.DefaultPictureFileKey, tempFilePath)
                    }).Build();

                DefaultPictureProvider provider = new DefaultPictureProvider(configuration);
                byte[] pictureBytes = provider.GetDefaultPicture();

                Assert.Multiple(() => {
                    Assert.That(File.Exists(tempFilePath));
                    Assert.That(pictureBytes, Is.EqualTo(expectedFile));
                });
            } finally {
                if(!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath)) {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}
