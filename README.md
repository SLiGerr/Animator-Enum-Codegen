# Animator-Enum-Codegen

- **What?** - Simple yet convinient way to bake all Animator Controllers to enums
- **Why?** - All the available tools that I found on github bake all states in string consts/single enum, which, it seems to me, are not convenient at all 

## Usage
0) (optional) Open ***"AnimatorStatesCodegen.cs"*** and edit name/path consts for your convenience
![image](https://github.com/SLiGerr/Animator-Enum-Codegen/assets/23235631/2b0d9201-3bd3-4fd6-bbcd-84c554fe319f)
2) Bake animators by pressing ***"Tools/Generate Animator States"***
3) Use enum/hashes in your code!
``` C#
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

        //Get Name by state
        var stateName = statesInfo.Names[(int)idleState];

        //Or use shortcuts!
        stateName = GetName(PlayerAvatarAnimator_States.Idle);
        
        //Get Hash by state
        var stateHash = statesInfo.Hashes[(int)idleState];
        
        //Or use shortcuts!
        stateHash = GetHash(PlayerAvatarAnimator_States.Idle);

        //Play from hash or from name
        animator.Play(stateHash);
        animator.Play(stateName);
    }
}
```
