using UnityEditor;
using UnityEngine;

namespace Packages.Replica.Bridge.Editor
{
    [InitializeOnLoad]
    [ExecuteAlways]
    public class Link
    {
        private static readonly Importer _importer = new Importer();
        private static readonly Connection _connection = new Connection();
        private static double NextConnectionCheckTime;

        static Link()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (!_connection.IsRunning())
            {
                var timeSinceStartup = EditorApplication.timeSinceStartup;
                if (timeSinceStartup >= NextConnectionCheckTime)
                {
                    NextConnectionCheckTime = timeSinceStartup + 10;
                    _connection.Connect();
                }
                return;
            };
            if (_connection.JsonCommands.Count <= 0) return;
            
            var json = _connection.JsonCommands[0];
            var command = JsonUtility.FromJson<Message>(json);
            if (command.IsCreateAsset())
            {
                var createAssetCommand = JsonUtility.FromJson<CreateAssetMessage>(json);
                var createTask = _importer.CreateAsset(createAssetCommand);
                do
                {
                } while (createTask.MoveNext());
                        
            }
            _connection.JsonCommands.RemoveAt(0);
        }
    }
}