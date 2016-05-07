using JustXaml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Services
{
    public class JumpListService
    {
        public async Task AddAsync(File value, int limit = 4)
        {
            if (Windows.UI.StartScreen.JumpList.IsSupported())
            {
                var jumplist = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
                RemoveExisting(value, jumplist);
                PruneListToLimit(limit, jumplist);
                var jumpItem = Windows.UI.StartScreen.JumpListItem.CreateWithArguments(value.Metadata, value.Title);
                jumplist.Items.Add(jumpItem);
                await jumplist.SaveAsync();
            }
        }

        public async Task<bool> ExistsAsync(File value)
        {
            return await ExistsAsync(value, await Windows.UI.StartScreen.JumpList.LoadCurrentAsync());
        }

        public async Task ClearAsync()
        {
            var jumplist = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            jumplist.Items.Clear();
            await jumplist.SaveAsync();
        }

        public async Task RemoveAsync(File value)
        {
            var jumplist = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            if (!await ExistsAsync(value, jumplist))
            {
                return;
            }
            var item = jumplist.Items.First(x => x.Arguments == value.Metadata);
            jumplist.Items.Remove(item);
            await jumplist.SaveAsync();
        }

        static void PruneListToLimit(int limit, Windows.UI.StartScreen.JumpList jumplist)
        {
            foreach (var item in jumplist.Items.Skip(limit).ToArray())
            {
                jumplist.Items.Remove(item);
            }
        }

        static void RemoveExisting(File value, Windows.UI.StartScreen.JumpList jumplist)
        {
            foreach (var item in jumplist.Items.Where(x => x.Arguments == value.Metadata))
            {
                jumplist.Items.Remove(item);
            }
        }

        async Task<bool> ExistsAsync(File value, Windows.UI.StartScreen.JumpList jumplist = null)
        {
            jumplist = jumplist ?? await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            return jumplist.Items.Any(x => x.Arguments == value.Metadata);
        }
    }
}
