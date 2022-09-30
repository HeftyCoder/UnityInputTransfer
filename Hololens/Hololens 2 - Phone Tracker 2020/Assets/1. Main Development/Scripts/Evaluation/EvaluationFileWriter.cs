using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using UOPHololens.Evaluation;
[CreateAssetMenu(menuName = "Evaluation File Writer")]
public class EvaluationFileWriter : ScriptableObject
{
    public string fileLocation;
    public string saveLocation;
    public int ageBiggerThan;
    public int targetSize = 20;

    public List<EvaluationPlayer> players = new List<EvaluationPlayer>();

    [ContextMenu("Deserialize Players")]
    public void DeserializePlayers()
    {
        players = GetPlayers();
    }

    // 1 native, 2 phone
    [ContextMenu("Extract to CSV")]
    public void ExtractToCSV()
    {
        var players = GetPlayers();
        using (var fs = new FileStream(saveLocation, FileMode.Create, FileAccess.Write))
        using (var s = new StreamWriter(fs))
        {
            s.WriteLine($"type,lookup, time, targets");
            foreach (var player in players)
            {
                var timeResponse = player.targetBasedTest;
                var targetReponse = player.timeBasedTest;
                s.WriteLine($"1,{timeResponse.FirstMeanLookUpTimeNative(targetSize)},{timeResponse.FirstMeanResponseTimeNative(targetSize)},{targetReponse.nativeTest[0].selectedCount}");
            }

            foreach (var player in players)
            {
                var timeResponse = player.targetBasedTest;
                var targetReponse = player.timeBasedTest;
                s.WriteLine($"2,{timeResponse.FirstMeanLookUpTimePhone(targetSize)},{timeResponse.FirstMeanResponseTimePhone(targetSize)},{targetReponse.phoneTest[0].selectedCount}");
            }
        }
    }
    public List<EvaluationPlayer> GetPlayers()
    {
        var result = new List<EvaluationPlayer>();

        var json = File.ReadAllText(fileLocation, System.Text.Encoding.UTF8);
        var jobj = JObject.Parse(json);
        var testers = jobj.AsJEnumerable().ToList();
        foreach (var tester in testers)
        {
            var first = tester.First();
            var player = JsonUtility.FromJson<EvaluationPlayer>(first.ToString());
            player.username = tester.Path;
            if (player.age > ageBiggerThan)
                result.Add(player);
        }

        return result;
    }
}
