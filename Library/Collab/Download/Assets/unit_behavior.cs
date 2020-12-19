using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class unit_behavior : MonoBehaviour
    {

        public int team;

        //behavioral state machine scripts
        public idle_component idleScript; //idle state
        public seek_target seekScript; //pursuing enemy state
        public attacking_behavior attackingScript;

        //misc stuff
        public GameObject selected_ui;
        public selection_component selectionScript; //script for when we are selected
        public draw_health healthScript; //script that draws a healthbar
        public flowfield_pathfinding ffp; //actually calculates the flowfields

        //gps is our general pathfinding script
        public general_pathfinding gps;

        //intelligent movement scripts
        public Agent agentScript;

        public Pursue pursue;
        public AvoidWall avoidWall;
        public FlowFieldSteering ffs;
        public Arrive arrive;
        public BoidSeparation boidSeparation;
        public BoidCohesion boidCohesion;
        public Seek seek;

        //tables
        public id_dictionary id_table;
        public ff_dictionary ff_table;
        public movement_group_dictionary mg_table;

        //lets us know if we are the leader of a movement group or not
        public bool isMgLeader = false;

        //
        public float maxHealth = 100.0f;
        public float health = 100.0f;
        public float viewDistance = 360.0f; //180 is probably a fine number
        public float shootDistance = 60.0f;
        public float maxSpeed;

        public GameObject target;

        public UnitFSM state;

        // Start is called before the first frame update
        void Start()
        {
            id_table = GameObject.Find("CustomEventSystem").GetComponent<id_dictionary>(); //holds the id's of all active gameobjects
            id_table.addObject(gameObject);
            ff_table = GameObject.Find("CustomEventSystem").GetComponent<ff_dictionary>(); //links GameObjects to their respective FF
            mg_table = GameObject.Find("CustomEventSystem").GetComponent<movement_group_dictionary>(); //links GO's to their movement groups

            agentScript = gameObject.AddComponent<Agent>(); //add agent
            agentScript.maxSpeed = maxSpeed;

            changeState(UnitFSM.Idle);
        }

    //
    private void Update()
    {
    }

    public enum UnitFSM //states
    {
        Attack,
        Seek,
        Idle
    }

    //allow other objects to change the state of the unit
    public void changeState(UnitFSM state)
    {

        switch (state)
        {
            case UnitFSM.Idle:

                if (gameObject.GetComponent<idle_component>() == null)
                {
                    idleScript = gameObject.AddComponent<idle_component>();
                }

                break;

            case UnitFSM.Seek:

                if (gameObject.GetComponent<seek_target>() == null)
                {
                    seekScript = gameObject.AddComponent<seek_target>();
                    seekScript.target = target;
                }

                break;

            case UnitFSM.Attack:

                if (gameObject.GetComponent<attacking_behavior>() == null)
                {
                    attackingScript = gameObject.AddComponent<attacking_behavior>();
                    attackingScript.target = target;
                }

                break;

        }

        this.state = state;

    }

    private void OnDestroy()
    {
        id_table.removeEntry(gameObject.GetInstanceID());
    }

}
