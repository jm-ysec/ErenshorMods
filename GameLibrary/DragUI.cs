// DragUI
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragUI : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public RectTransform Parent;

	private bool dragging;

	private Vector3 pos;

	public bool isInv;

	private Vector2 offset;

	public RectTransform MyAnchor;

	public Vector2 PrefPos;

	private Image MyImg;

	public List<Image> Borders;

	private RectTransform _parentRT;

	private Vector2 _dragDelta;

	private static readonly Vector3[] _corners = new Vector3[4];

	private RectTransform rt;

	private void Awake()
	{
		if (GameData.AllUIElements != null && !GameData.AllUIElements.Contains(this))
		{
			GameData.AllUIElements.Add(this);
		}
		rt = GetComponent<RectTransform>();
		MyImg = GetComponent<Image>();
	}

	private void Start()
	{
		if (Borders == null)
		{
			Borders = new List<Image>();
		}
		if (MyAnchor != null)
		{
			Parent.anchorMin = MyAnchor.anchorMin;
			Parent.anchorMax = MyAnchor.anchorMax;
			Parent.pivot = MyAnchor.pivot;
			Parent.anchoredPosition = MyAnchor.anchoredPosition;
		}
		_parentRT = (RectTransform)Parent.parent;
		RectTransform rectTransform = (RectTransform)base.transform;
		offset = rectTransform.anchoredPosition - Parent.anchoredPosition;
		pos = Parent.anchoredPosition - offset;
		if (PlayerPrefs.HasKey(base.transform.name + "x") && PlayerPrefs.HasKey(base.transform.name + "y"))
		{
			if (!isInv)
			{
				Parent.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(base.transform.name + "x"), PlayerPrefs.GetFloat(base.transform.name + "y"));
			}
			else
			{
				Parent.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(base.transform.name + "x") - offset.x, PlayerPrefs.GetFloat(base.transform.name + "y") - offset.y);
			}
		}
		PlayerPrefs.SetFloat(base.transform.name + "x", Parent.anchoredPosition.x);
		PlayerPrefs.SetFloat(base.transform.name + "y", Parent.anchoredPosition.y);
		PrefPos = Parent.anchoredPosition;
		if (IsOffScreen() && GameData.AutorecoverUI)
		{
			Restore();
		}
	}

	private void Update()
	{
		if (!GameData.EditUIMode)
		{
			if (!MyImg.enabled)
			{
				return;
			}
			MyImg.enabled = false;
			if (Borders.Count <= 0)
			{
				return;
			}
			{
				foreach (Image border in Borders)
				{
					if (border.enabled)
					{
						border.enabled = false;
					}
				}
				return;
			}
		}
		if (MyImg.enabled)
		{
			return;
		}
		MyImg.enabled = true;
		if (Borders.Count <= 0)
		{
			return;
		}
		foreach (Image border2 in Borders)
		{
			if (!border2.enabled)
			{
				border2.enabled = true;
			}
		}
	}

	private void LateUpdate()
	{
		if (dragging && RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRT, Input.mousePosition, null, out var localPoint))
		{
			Parent.anchoredPosition = localPoint + _dragDelta;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			dragging = true;
			GameData.DraggingUIElement = true;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRT, eventData.position, null, out var localPoint))
			{
				_dragDelta = Parent.anchoredPosition - localPoint;
			}
			else
			{
				_dragDelta = Vector2.zero;
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		dragging = false;
		PlayerPrefs.SetFloat(base.transform.name + "x", Parent.anchoredPosition.x);
		PlayerPrefs.SetFloat(base.transform.name + "y", Parent.anchoredPosition.y);
		GameData.DraggingUIElement = false;
		PrefPos = Parent.anchoredPosition;
	}

	public void Restore()
	{
		dragging = false;
		Canvas.ForceUpdateCanvases();
		if (MyAnchor != null)
		{
			LayoutGroup component = Parent.GetComponent<LayoutGroup>();
			ContentSizeFitter component2 = Parent.GetComponent<ContentSizeFitter>();
			bool flag = false;
			bool flag2 = false;
			if ((bool)component)
			{
				flag = component.enabled;
				component.enabled = false;
			}
			if ((bool)component2)
			{
				flag2 = component2.enabled;
				component2.enabled = false;
			}
			Parent.pivot = MyAnchor.pivot;
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, MyAnchor.TransformPoint(MyAnchor.rect.center));
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)Parent.parent, screenPoint, null, out var localPoint))
			{
				if (Parent.anchorMin != Parent.anchorMax)
				{
					Vector3 localPosition = new Vector3(localPoint.x, localPoint.y, 0f);
					Parent.localPosition = localPosition;
				}
				else
				{
					Parent.anchoredPosition = localPoint;
				}
			}
			Vector3 localPosition2 = Parent.localPosition;
			Parent.localPosition = new Vector3(localPosition2.x, localPosition2.y, 0f);
			if ((bool)component)
			{
				component.enabled = flag;
			}
			if ((bool)component2)
			{
				component2.enabled = flag2;
			}
		}
		PlayerPrefs.SetFloat(base.transform.name + "x", Parent.anchoredPosition.x);
		PlayerPrefs.SetFloat(base.transform.name + "y", Parent.anchoredPosition.y);
		GameData.DraggingUIElement = false;
		PrefPos = Parent.anchoredPosition;
	}

	public bool IsOffScreen(float overshootPx = 0f)
	{
		Canvas canvas = rt.GetComponentInParent<Canvas>()?.rootCanvas;
		if (canvas == null)
		{
			return false;
		}
		Canvas.ForceUpdateCanvases();
		Rect rect = new Rect(canvas.pixelRect.xMin - overshootPx, canvas.pixelRect.yMin - overshootPx, canvas.pixelRect.width + overshootPx * 2f, canvas.pixelRect.height + overshootPx * 2f);
		rt.GetWorldCorners(_corners);
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Vector2 point = RectTransformUtility.WorldToScreenPoint(null, _corners[i]);
			if (!rect.Contains(point))
			{
				num++;
			}
		}
		return num == 4;
	}
}
