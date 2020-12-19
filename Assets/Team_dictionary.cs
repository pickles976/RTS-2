using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team_dictionary : MonoBehaviour
{

    //
    Dictionary<int,team_class> team_dict;

    //
    private void Start()
    {
        team_class team0 = new team_class(0,0);
        team_class team1 = new team_class(1, 0);

        team_dict = new Dictionary<int, team_class>();

        team_dict.Add(0, team0);
        team_dict.Add(1, team1);

    }

    //
    public team_class getTeamData(int team_num)
    {
        if (team_dict.ContainsKey(team_num))
        {
            return team_dict[team_num];
        }

        return null;

    }
}
