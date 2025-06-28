using DevHabit.APi.DTOs.Common;

namespace DevHabit.APi.DTOs.Tags
{
    public class TagsCollectionDto: ICollectionResponse<TagDto>
    {
        public List<TagDto> Items { get; init; }
    }
}