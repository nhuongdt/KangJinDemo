Vue.component('dropdown-search', {
    template: `<div class="gara-bill-infor-button shortlabel">
        <div v-show="showbutton">
            <a>
                <i class="material-icons">add</i>
            </a>
        </div>
        <input class="gara-search-HH" v-bind:placeholder="GetPlaceholder()" v-model="text" v-on:keydown="onKeyDown" v-on:focusout="onFocusout" />
        <div class="gara-search-dropbox drop-search">
            <ul>
                <li v-for="item in listdata" v-on:click="SelectValue(item)">
                    <a href="javascript:void(0)">
                        <span class="tit-seach" v-if="colshow >= 2" >{{ item.val1 }}</span>
                        <span class="seach-hh" v-if="colshow >= 1">{{ item.val2 }}</span>
                        <span v-if="colshow >= 3">{{ item.val3 }}</span>
                    </a>
                </li>
            </ul>
        </div>
    </div>`,
    methods:
    {
        GetPlaceholder: function () {
            return this.placeholder;
        },
        FilterTextSearch: function () {
            var searchkey = this.text;
            this.$emit('onsearch', { searchkey });
        },
        SelectValue: function (value) {
            var avs = $(this)[0].$el;
            avs.getElementsByClassName('gara-search-dropbox')[0].style.display = 'none';
            var id = value.ID;
            if (id === this.valueselected) {
                this.GetText();
            }
            else {
                this.$emit('onselectvalue', { id });
            }
        },
        onKeyDown: function (event) {
            this.setFromFocus = false;
            if (event.keyCode === 13) {
                if (this.listdata[0] !== undefined) {
                    this.SelectValue(this.listdata[0]);
                }
            }
            else {
                $(this)[0].$el.getElementsByClassName('gara-search-dropbox')[0].style.display = 'block';
            }
        },
        onFocusout: function () {
            this.setFromFocus = true;
            this.text = this.textold;
        },
        GetText: function () {
            this.setFromFocus = false;
            var obj = this.listdata.find(p => p.ID === this.valueselected);
            if (obj !== undefined) {
                this.text = obj.val2;
            }
            else {
                this.text = '';
            }
            this.textold = this.text;
        }
    },
    props: ['showbutton', 'placeholder', 'listdata', 'colshow', 'valueselected'],
    data: function (){
        return {
            text: '',
            textold: '',
            setFromFocus: false
        }
    },
    watch: {
        text: function () {
            if (!this.setFromFocus) {
                this.FilterTextSearch();
            }
            else {
                this.setFromFocus = false;
            }
        },
        valueselected: function () {
            this.GetText();
        },
    }
});