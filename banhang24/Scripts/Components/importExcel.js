var cpmImportExcel = {
    props: {
        title: { default: 'Nhập file excel' },
        isChosingFile: { default: false },
        isShowCheckboxPassErr:{ default: true },
        ListChose: { default: [] },
        ListError: { default: [] },
    },
    template: `
    <div class="modal-dialog draggable modal-lg">
        <div class="modal-content ">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <i class="material-icons">close</i>
                </button>
                <h4 class="modal-title">{{title}}</h4>
            </div>
            <div class="modal-body">
                <div>
                    <div class="col-sm-12">
                        <ul class="lstRdo">
                            <li v-for="(item, index) in ListChose">
                                <label>
                                    <input type="radio" name="rdoImport" value="true" 
                                        v-model="item.Value" v-on:change="ChangeTypeUpdate(item)">
                                    {{item.Text}}
                                </label>
                            </li>
                        </ul>
                        <div class="floatleft excel-file nopadding">
                            <div v-if="!isChosingFile">
                                <h5 class="floatleft">Nhập thông tin từ file Excel</h5>
                                <div class="title-inport ">
                                    Tải về file mẫu
                                    <span class="blue " v-on:click="DownloadFile(1)"> Excel 2003</span>
                                    hoặc<span class="blue" v-on:click="DownloadFile(0)"> bản cao hơn</span>
                                </div>
                            </div>
                            <div class="choose-file">
                                <span class="btn btn-import btn-file form-control no-magrin " v-if="!isChosingFile">
                                    Chọn file dữ liệu 
                                    <input type='file' name="image"  v-on:change="ChoseFile"
                                    accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" />
                                </span>
                                <div class="padingbottom" v-if="isChosingFile">
                                    <div class="list-file flex flex-row">
                                        <ul style="width:calc(100% - 50px)">
                                            <li>
                                                <span>{{FileChosed.FileName}}</span>
                                                <i class="far fa-times-circle fa-2x" style="color:red" v-on:click="DeleteFile"></i>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                                <div v-if="isChosingFile">
                                    <button class="btn btn-main" v-on:click="Import">
                                        <i class="fa fa-floppy-o"></i> Thực hiện
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div class="warning-content floatleft" v-if="ListError.length===0">
                            <h4 class="title-alert"><i class="fa fa-warning"></i> Lưu ý</h4>
                            <p>Bạn vui lòng không thay đổi định dạng hàng cột theo file Excel mẫu .</p>
                            <p>Hệ thống cho phép nhập tối đa <span> 8000</span> dòng mỗi lần từ file Excel 2003 hoặc bản cao hơn .</p>
                            <p  v-if="isShowCheckboxPassErr">Click vào checkbox "đồng ý..." nếu bạn muốn tiếp tục import dữ liệu bằng cách bỏ qua dữ liệu sai.</p>
                        </div>
                    </div>
                </div>
                <div v-if="ListError.length > 0"> 
                    <div class="error-data">
                        <h4 class="bold">Bảng thông báo lỗi</h4>
                    </div>
                    <div class="table-reponsive table_price table-frame">
                        <table class="table  table-hover table-wh">
                            <thead>
                                <tr>
                                    <th>Tên trường dữ liệu</th>
                                    <th>Vị trí</th>
                                    <th>Thuộc tính</th>
                                    <th>Diễn giải</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in ListError">
                                    <td>{{item.TenTruongDuLieu}}</td>
                                    <td class="text-center">{{item.ViTri}}</td>
                                    <td>{{item.ThuocTinh}}</td>
                                    <td>{{item.DienGiai}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer file-all">
                        <div class="flex flex-between">
                            <div class="form-group pull-left continue-hh ">
                                <label style="width:unset" v-if="isShowCheckboxPassErr">
                                    <input type="checkbox" value="true" v-model="passError" />
                                    <span>  Đồng ý bỏ qua <a></a> lỗi và tiếp tục</span>
                                </label>
                            </div>
                             <div class="flex flex flex-end">
                            <button type="button" class="btn btn-main" v-if="isShowCheckboxPassErr" v-bind:disabled="!passError" v-on:click="PassErr_Continue">
                                <i class="fa fa-save"></i> Tiếp tục
                            </button>
                            <button data-dismiss="modal" class="btn btn-cancel">
                               <i class="fa fa-ban"></i> Bỏ qua
                              </button>
                              </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

`,
    data: function () {
        return {
            FileChosed: {},
            passError: false,
        }
    },
    methods: {
        DownloadFile: function (type = 0) {
            var self = this;
            self.$emit('download-file', type);
        },
        ChoseFile: function () {
            var self = this;
            self.isChosingFile = true;
            self.passError = false;
            self.ListError = [];

            var files = event.target.files;
            for (let k = 0; k < files.length; k++) {
                let f = files[k];
                let reader = new FileReader();
                reader.onload = (function (thisFile) {
                    return function (e) {
                        self.FileChosed = {
                            file: thisFile,
                            URLAnh: e.target.result,
                            FileName: thisFile.name
                        };
                    };
                })(f);
                reader.readAsDataURL(f);
            }
        },
        DeleteFile: function () {
            var self = this;
            self.FileChosed = {};
            self.isChosingFile = false;
        },
        ChangeTypeUpdate: function (item) {
            var self = this;
            self.$emit('change-type-update', item);
        },
        Import: function () {
            var self = this;
            self.$emit('import-file', self.FileChosed);
        },
        PassErr_Continue: function () {
            var self = this;
            self.$emit('import-file', self.FileChosed);
        }
    },
}

var cmpYesNo = {
    props: {
        title: { default: 'Xác nhận cập nhật' },
        mes: { default: '' },
        show: { default: false },
        ListChose: { default: [] },
    },
    template: `
 <div class="modal fade">
    <div class="modal-dialog draggable modal-sml">
        <div class="modal-content ">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <i class="material-icons">close</i>
                </button>
                <h4 class="modal-title">{{title}}</h4>
            </div>
            <div class="modal-body">
            <div> 
                 {{mes}}
            </div>
                <div>
                    <div class="col-sm-12">
                        <ul class="lstRdo" style="float:left">
                            <li v-for="(item, index) in ListChose">
                                <label>
                                    <input type="checkbox" name="cmpCheckList" value="true" 
                                        v-model="item.Value" v-on:change="ChoseItem(item)">
                                    {{item.Text}}
                                </label>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
                <div class="modal-footer">
                        <button type="button" data-dismiss="modal" class="btn btn-cancel">
                        <i class="fa fa-ban"></i>Hủy
                        </button> 
                        <button type="button" class="btn btn-save" v-on:click="Agree"><i class="fa fa-save"></i>Đồng ý
                        </button>
                </div>
        </div>
    </div>
</div>
`,
    data: function () {
        return {
            arrChosed: [],
        }
    },
    methods: {
        ChoseItem: function (item) {
            var self = this;
            self.arrChosed.push(item);
        },
        Agree: function () {
            var self = this;
            self.arrChosed = self.ListChose.filter(x => x.Value);
            self.$emit('agree', self.arrChosed);
        }
    },
    watch: {
        show: function (val) {
            if (val) {
                this.arrChosed = [];
            }
        }
    }
}