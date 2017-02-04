

using System;
using System.IO;
using Xamarin.Forms;
using XamFormsReactiveUI.DataLayer;
using XamFormsReactiveUI.Droid;

[assembly: Dependency(typeof(SqLiteDroid))]
namespace XamFormsReactiveUI.Droid
{

    public class SqLiteDroid : ISQLite
    {
        public SQLite.SQLiteAsyncConnection GetConnection()
        {
            const string sqliteFilename = "app.db";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            // This is where we copy in the prepopulated database
            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                var s = Forms.Context.Resources.OpenRawResource(Resource.Raw.app);  // RESOURCE NAME ###

                // create a write stream
                var writeStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                // write to the stream
                ReadWriteStream(s, writeStream);
            }

            var conn = new SQLite.SQLiteAsyncConnection(path);

            // Return the database connection 
            return conn;
        }

        /// <summary>
        /// helper method to get the database out of /raw/ and into the user filesystem
        /// </summary>
        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            const int length = 256;
            var buffer = new byte[length];
            var bytesRead = readStream.Read(buffer, 0, length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }

}