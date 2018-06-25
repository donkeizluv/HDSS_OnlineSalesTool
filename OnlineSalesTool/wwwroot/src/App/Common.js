export default {
    langCode: 'vn',
    errorMessages: {
        'vn': {
            '404': '404 Không tìm thấy',
            '401': '401 Không được phép',
            '403': '403 Không được phép',
            '500': '500 Lỗi hệ thống',
            '504': '504 Hết thời gian chờ'
        }
    },
    errorCodeTranslater(code) {
        return this.errorMessages[this.langCode][code];
    }
}
