using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        //初始化animation clip和animationType键值对
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        //初始化string 和animationType键值对
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    public void ApplyCharacterCustomisationParamters(List<CharacterAttribute> characterAttributeList)
    {
        //遍历所有character attributes并设置重载的动画控制器
        foreach (CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            //当前动画和替代动画的键值对
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            //在场景中搜索动画器以匹配scriptable object animator type
            Animator[] animatorArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            //Get base animations for animator 
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationList = new List<AnimationClip>(aoc.animationClips);//通过overridecontroller检索控制器下所有animationClip，并创建animationClip列表

            foreach (AnimationClip animationClip in animationList)
            {
                //find animation in dictionary
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)//如果找到，该动画需要被替换为响应其他动画
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            //apply animation updates to animation override controller and then update animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
