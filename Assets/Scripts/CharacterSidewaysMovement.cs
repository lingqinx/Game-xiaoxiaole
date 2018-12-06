using UnityEngine;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSidewaysMovement : MonoBehaviour
{


	private Vector3 moveDirection = Vector3.zero;//移动方向
	public float gravity = 20f;//y轴的参数
	private CharacterController controller;//控制小车

	private bool isChangingLane = false;//是否变换跑道
	private Vector3 locationAfterChangingLane;//变换车道后的距离
    //distance character will move sideways
	private Vector3 sidewaysMovementDistance = Vector3.right * 2f;//小车移动的车道的距离

	public float SideWaysSpeed = 5.0f;//跑道速度

	public float JumpSpeed = 8.0f;//跳高的速度
	public float Speed = 6.0f;//小车速度
    //Max gameobject
	public Transform CharacterGO;// 小车
    
	IInputDetector inputDetector = null;//检测输入动作

    // Use this for initialization
    void Start()
    {
        moveDirection = transform.forward;
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= Speed;

        UIManager.Instance.ResetScore();
        UIManager.Instance.SetStatus(Constants.StatusTapToStart);

        GameManager.Instance.GameState = GameState.Start;

        //anim = CharacterGO.GetComponent<Animator>();
        inputDetector = GetComponent<IInputDetector>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.Start:
			//游戏开始时的状态
                if (Input.GetMouseButtonUp(0))
                {
                    var instance = GameManager.Instance;
                    instance.GameState = GameState.Playing;

                    UIManager.Instance.SetStatus(string.Empty);
                }
                break;
            case GameState.Playing:
			//玩游戏的状态处理
                //UIManager.Instance.IncreaseScore(1f);

                CheckHeight();

                DetectJumpOrSwipeLeftRight();

			//计算高度
                moveDirection.y -= gravity * Time.deltaTime;

                if (isChangingLane)
			{//判断是否移动到旁边的跑道
                    if (Mathf.Abs(transform.position.x - locationAfterChangingLane.x) < 0.1f)
                    {
                        isChangingLane = false;
                        moveDirection.x = 0;
                    }
                }

			//移动小车到当前位置
                controller.Move(moveDirection * Time.deltaTime);

                break;
			//游戏结束
            case GameState.Dead:
                
                if (Input.GetMouseButtonUp(0))
                {
				//重新开始
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            default:
                break;
        }

    }
	//检测小车高度，判断是否跳
    private void CheckHeight()
    {
        if (transform.position.y < -10)
        {
            GameManager.Instance.Die();
        }
    }
	//查看当前动作：左滑，右滑，跳高
    private void DetectJumpOrSwipeLeftRight()
    {
        var inputDirection = inputDetector.DetectInputDirection();
		//如果动作为跳
        if (controller.isGrounded && inputDirection.HasValue && inputDirection == InputDirection.Top
            && !isChangingLane)
        {
            moveDirection.y = JumpSpeed;
		}//动作为左滑或右滑
        if (controller.isGrounded && inputDirection.HasValue && !isChangingLane)
        {
            isChangingLane = true;
			//左滑，往左变更跑道及相应距离
            if (inputDirection == InputDirection.Left)
            {
                locationAfterChangingLane = transform.position - sidewaysMovementDistance;
                moveDirection.x = -SideWaysSpeed;
			}//右滑，往右变更跑道，加上移动距离
            else if (inputDirection == InputDirection.Right)
            {
                locationAfterChangingLane = transform.position + sidewaysMovementDistance;
                moveDirection.x = SideWaysSpeed;
            }
        }


    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
		//判断是否已经到了边界
        if(hit.gameObject.tag == Constants.WidePathBorderTag)
        {
            isChangingLane = false;
            moveDirection.x = 0;
        }
    }

    

}
