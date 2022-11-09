Vue.component('treeview', {
    template: `
                <ul v-if="list.length > 0">
                    <li v-for="item in list" class="ss-li">
                        <div class="li-top li-oo" v-on:click="SelectValue(item.Item.ID)">
                            <span class="li-top">{{ item.Item.Text }}</span>
                        </div>
                        <span class="close-ul" v-on:click="Expand" v-show="item.Children.length >0 ">
                            <i class="fa fa-plus"></i>
                        </span>
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

// treeview search
var cmpTreeView = {
    props: {
        listAll: {
            default: []
        },
        listSearch: {
            default: []
        },
        formType: { default: 0 },// 1.chuyennhom, 0.other
        showAll: { default: false },
        isCheck: { default: false },
        isRdo: { default: false },
        roleEdit: { default: false },
        idChosing: { default: null },
    },
   
    template: `  <div>
                            <div class="input-group"  v-if="formType == 0">
                               <div class="input-group-addon">
	                            <span> <i class="fa fa-search"></i>
                                </span> 
                               </div>
                               <input class="form-control"  type="text" 
                                v-model="txtSearch"
                                v-on:keyup="search(listAll)"/>
                          </div>
                           <div class="col-sm-12 col-lg-12 col-md-12 form-group" v-if="formType == 1">
                                <label> Chuyển đến: 
                                </label>
                                <input class="form-control" readonly  type="text"   v-model="txtSearch" />
                           </div>

                     <ul>
                       <li class="treeview-li" v-on:click="choseItem" v-if="showAll"
                             v-bind:style="[idChosing == null ? {'background':'#eee'} : {'background':'none'}]">
                            <div class="group-p1" style="padding-top: 5px;padding-bottom: 5px">
                                <label class="treeview-a" href="javascript:void(0)"> Tất cả</label>
                            </div>
                        </li>
                        <li v-for="(item,index) in listSearch"
                            v-bind:style="[idChosing === item.id ? {'background':'#eee'} : {'background':'none'}]">
                             <div class="col-sm-12 col-lg-12 col-md-12 group-p1">
                                <div class="form-check flex">
                                  <input class="form-check-input" type="checkbox" v-if="isCheck" >
                                  <input class="form-check-input" type="radio" name="rdoTree"  
                                        v-if="isRdo"
                                        v-bind:checked="item.id == idChosing">
                                  <label class="form-check-label" for="gridCheck" v-on:click="choseItem(item,1)"> {{item.text}} </label>                     
                                  <a v-if="roleEdit" v-on:click="editItem(item,1)"><i class="fa fa-edit"> </i> </a>
                                </div> 
                                    <ul>
                                        <li v-for="(item2,index2) in item.children"
                                            >
                                            <div class="col-sm-12 col-lg-12 col-md-12 group-p2">
                                                <div class="form-check flex">
                                                  <input class="form-check-input" type="checkbox"
                                                         v-if="isCheck"> 
                                                  <input class="form-check-input" type="radio" name="rdoTree"
                                                          v-if="isRdo"
                                                          v-bind:checked="item2.id == idChosing">
                                                  <label class="form-check-label" for="gridCheck" v-on:click="choseItem(item2,2)"> {{item2.text}} </label>
                                                   <a v-if="roleEdit" v-on:click="editItem(item2,2)"><i class="fa fa-edit"> </i> </a>
                                                </div> 
                                            </div>      
                                            <ul>
                                                <li v-for="(item3,index3) in item2.children"
                                                     v-on:click="choseItem(item3,3)">
                                                     <div class="col-sm-12 col-lg-12 col-md-12 group-p3">
                                                        <div class="form-check">
                                                         <input class="form-check-input" type="checkbox"
                                                             v-if="isCheck">
                                                          <input class="form-check-input" type="radio" name="rdoTree"
                                                                v-if="isRdo"
                                                                v-bind:checked="item3.id == idChosing">
                                                          <label class="form-check-label" for="gridCheck">
                                                           {{item3.text}}
                                                          </label>
                                                     </div> 
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                            </div>
                        </li>
                    </ul>
                  </div>
                    `,
    data: function () {
        return {
            txtSearch: '',
        }
    },
    methods: {
        search: function (arr) {
            let self = this;
            let text = self.txtSearch;
            self.listSearch = arr.filter(function (o) {
                if (o.children) o.children = self.search(o.children);
                return commonStatisJs.convertVieToEng(o.text).match(text);
            })
        },
        choseItem: function (item = null, type) {
            let self = this;
            if (item !== null) {
                self.txtSearch = item.text;
                self.idChosing = item.id;
            }
            else {
                self.txtSearch = '';
                self.idChosing = null;
            }
            switch (type) {
                case 2:
                    break;
            }
            self.$emit('chose-item', item);
        },
        editItem: function (item, type) {
            let self = this;
            self.$emit('edit-item', item);
        }
    },
}

