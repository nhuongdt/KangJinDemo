Vue.component('filter-checkbox', {
    template: `<aside class="op-filter-container">
                            <div class="menuCheckbox">
                                <ul>
                                    <li v-for="item in listitem">
                                        <label>
                                            <input type="checkbox" v-bind:value="item.Value" v-bind:checked="item.Checked" @click="CheckboxChange(item.Value)" /> {{item.Text}}
                                        </label>
                                    </li>
                                </ul>
                            </div>
                        </aside>`,
    props: ['listitem'],
    methods:
    {
        CheckboxChange: function (valselect) {
            let itemupdate = this.listitem.find(p => p.Value === valselect);
            if (itemupdate.Checked === true) {
                itemupdate.Checked = false;
            }
            else {
                itemupdate.Checked = true;
            }
            this.$emit('callfunctionloaddata');
        }
    }
})