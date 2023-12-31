# Animator-Enum-Codegen

- **What?** - Simple yet convenient way to bake all Animator Controllers to enums
- **Why?** - All the available tools that I found on github bake all states in string consts/single enum, which, it seems to me, are not convenient at all 

## Download

1) Open ```Package Manager``` in Unity,
2) Select ```Add package from git URL...```,
3) Paste ```https://github.com/SLiGerr/Animator-Enum-Codegen.git``` and press **Add**

## Usage
0) Create config by pressing ***"Tools/Animator-Enum-Codegen/Create Config"*** and edit name/path fields for your convenience
   <blockquote> 
   <details>
   <summary>Config Fields</summary>
   
   ![image](https://github.com/SLiGerr/Animator-Enum-Codegen/assets/23235631/0be96aca-1161-4e11-8d31-f181a6e9b70b)
   
   </details>
   </blockquote>
1) Bake animators by pressing ***"Tools/Animator-Enum-Codegen/Generate Animator States"***, result:
   <blockquote> 
   <details>
   <summary>Generation Example</summary>

   ![image](https://github.com/SLiGerr/Animator-Enum-Codegen/assets/23235631/f48e87e8-0ac5-4083-be9c-316303de9f92)

   </details>
   </blockquote>
    

2) Use enum/hashes in your code!

```CCharp
using static GeneratedData.AnimatorParams; //usage shortcut 

public class Test : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private void Foo()
    {
        //Get enum state of any controller
        var idleState = PlayerAvatarAnimator_States.Idle;

        //Get info by the type 
        var statesInfo = Infos[typeof(PlayerAvatarAnimator_States)];

        //Get Name, Hash or Duration by state
        var stateName      = statesInfo.Names[(int)idleState];
        var stateHash      = statesInfo.Hashes[(int)idleState];
        var stateDuration  = statesInfo.Durations[(int)idleState];

        //Or use shortcuts!
        stateName     = GetName(idleState);
        stateHash     = GetHash(idleState);
        stateDuration = GetDuration(idleState);
        
        //Play from hash or from name
        animator.Play(stateHash);
        animator.Play(stateName);
        
        Debug.Log($"{stateName} duration is {stateDuration}");
    }
}