Vue.component('treeview', {
    template: `
                <ul v-if="list.length > 0">
                    <li v-for="item in list" class="ss-li">
                        <div class="li-top li-oo" v-on:click="SelectValue(item.Item.ID)">
                            <span class="li-top">{{ item.Item.Text }}</span>
                        </div>
                        <span class="close-ul" v-on:click="Expand" v-show="item.Children.length >0 ">+</span>
                        <treeview v-if v-bind:list="item.Children" v-bind:onselectvalue="onselectvalue"></treeview>
                    </li>
                </ul>`,
    props: {
        list: [],
        onselectvalue: null
    },
    methods: {
        Expand: function (e) {
            $(e.target).next("ul").toggleClass("open");
            if ($(e.target).next("ul").hasClass("open")) {
                $(e.target)[0].innerHTML = '-';
            }
            else {
                $(e.target)[0].innerHTML = '+';
            }
        },
        SelectValue: function (value) {
            this.onselectvalue(value);
        },
    }
})