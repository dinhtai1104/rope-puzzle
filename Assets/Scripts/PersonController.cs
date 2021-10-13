using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PersonController : MonoBehaviour
{
    [SerializeField] Rigidbody2D myRb;
    [SerializeField] Animator myAnimator;
    //[SerializeField] float speedMove;
    [SerializeField] float speedOnZipline;
    [SerializeField] bool isRescued = false;
    [SerializeField] bool isSwing = false;
    [SerializeField] SkinPerson skin;
    [SerializeField] GameObject colliderBody;

    public Color[] skinColors;
    private List<Vector2> path;
    private Vector3 leftHouse, rightHouse, leftCar, rightCar;
    private Vector3 left, right;
    private bool isAlive = true;
    // For random movement
    private int direction = 0;
    private float speedMoveLeftRight = 0;
    private float timeSwap = 0;
    private float timeControl = 0;
    private int index = 0;
    private void OnEnable()
    {
        index = 0;
        skin.SetColor(skinColors[Random.Range(0, skinColors.Length)]);
        colliderBody.SetActive(true);
        myAnimator.SetFloat("timescale", Random.Range(0.5f, 1.5f));
        direction = Random.value > 0.5f ? -1 : 1;
        speedMoveLeftRight = Random.Range(0.4f, 0.7f);
        timeSwap = Random.Range(2.0f, 4f);
        timeControl = Time.time;
        isAlive = true;
        isRescued = false;
        isSwing = false;
        myAnimator.SetBool("swing", isSwing);
    }
    public void SetColorSkin(Color color)
    {
        skin.SetColor(color);
    }
    public void InitPerson(Vector3 leftHouse, Vector3 rightHouse, Vector3 leftCar, Vector3 rightCar)
    {
        this.leftHouse = leftHouse;
        this.rightHouse = rightHouse;
        this.leftCar = leftCar;
        this.rightCar = rightCar;
        left = leftHouse;
        right = rightHouse;
    }


    private void Update()
    {
        if (!isSwing && gameObject.layer == LayerMask.NameToLayer("Rescue"))
        {
            HandleMovement();
        }
    } 

    private void HandleMovement()
    {
        float distance = Mathf.Abs(0.05f);
        float step = direction * speedMoveLeftRight * Time.deltaTime;
        if (direction == -1)
        {
            if (transform.position.x + step * 1.2f <= left.x)
            {
                direction = 1;
                speedMoveLeftRight = Random.Range(0.4f, 0.7f);
                timeSwap = Random.Range(2.0f, 4f);
            }
            else
            {
                if (Time.time - timeControl > timeSwap)
                {
                    timeControl = Time.time;
                    direction *= -1;
                    speedMoveLeftRight = Random.Range(0.4f, 0.7f);
                    timeSwap = Random.Range(2.0f, 4f);
                }
            }
        } else
        {
            if (transform.position.x + step * 1.2f >= right.x)
            {
                direction = -1;
                speedMoveLeftRight = Random.Range(0.4f, 0.7f);
                timeSwap = Random.Range(2.0f, 4f);
            } else
            {
                if (Time.time - timeControl > timeSwap)
                {
                    timeControl = Time.time;
                    direction *= -1;
                    speedMoveLeftRight = Random.Range(0.4f, 0.7f);
                    timeSwap = Random.Range(2.0f, 4f);
                }
            }
        }
        transform.Translate(direction * speedMoveLeftRight * Time.deltaTime, 0, 0);
    }

    public void StartZipline(List<Vector2> points)
    {
        if (isRescued) return;
        this.path = points;
        StartCoroutine(OnZipline());
    }

    private IEnumerator OnZipline()
    {
        isSwing = true;
        myAnimator.SetBool("swing", isSwing);
        myRb.isKinematic = true;
        while(index < path.Count && isAlive)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[index], speedOnZipline * Time.deltaTime);
            if (Vector2.Distance(transform.position, path[index]) < 0.05f)
            {
                transform.position = path[index];
                index++;
            }
            yield return null;
        }
        if (isAlive)
        {
            myAnimator.SetFloat("timescale", Random.Range(0.5f, 1.5f));
            isSwing = false;
            isRescued = true;
            myRb.isKinematic = false;
            myAnimator.SetBool("swing", isSwing);
            left = leftCar;
            right = rightCar;
            transform.DOMove(transform.position + Vector3.down * 0.5f, 0f);
            transform.DOMove( transform.position + Vector3.right * GameController.Instance.GetDirection() ,0.5f );
            //Call Event when rescue touch ground
            GameMaster.onNotifyRescuedPeople?.Invoke();
        }
    }

    public bool IsFinish()
    {
        return (isRescued && !isSwing) || (!isAlive);
    }

    public void ChangeLayer()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Rescue");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("saw"))
        {
            if (isAlive && gameObject.layer != LayerMask.NameToLayer("Rescue2"))
            {
                colliderBody.SetActive(false);
                isAlive = false;
                myRb.isKinematic = false;
                myRb.AddForce(new Vector2(Random.Range(-1, 1), 2) * 10, ForceMode2D.Impulse);
                myAnimator.SetBool("swing", false);
            }
        } 
        else if (collision.CompareTag("boundary"))
        {
            gameObject.SetActive(false);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rescue2"))
        {
            PersonController rescue = collision.GetComponentInParent<PersonController>();
            if (rescue != null)
            {
                rescue.transform.eulerAngles = Vector3.zero;
                rescue.gameObject.layer = LayerMask.NameToLayer("Rescue");
                rescue.ChangeLayer();
                rescue.index = this.index;
                rescue.transform.position = transform.position;
                rescue.InitPerson(leftHouse, rightHouse, leftCar, rightCar);
                StartCoroutine(waitAction(() => { rescue.StartZipline(WrappingRope.RopeSystem.Instance.getPath()); }, 0.1f));
                
            } else
            {
                Debug.Log("Errr");
            }
        }
    }
    IEnumerator waitAction(System.Action ac, float time)
    {
        yield return new WaitForSeconds(time);
        ac?.Invoke();
    }

}
