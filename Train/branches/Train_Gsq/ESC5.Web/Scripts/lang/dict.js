//固定的字典信息
(function (String, angular) {
    'use strict';

    angular.module('projectbase').constant("App_Dict", {

        "zh-cn": {
            TrueDisplay: '是',
            FalseDisplay: '否',

            TrueFalseEnum: {
                '否': false,
                '是': true
            },
            User_RankEnum: {
                '1级': 1,
                '2级': 2,
                '3级': 3,
                '4级': 4,
            },
            Task_StatusEnum: {
                '预备': 1,
                '正在进行': 2,
                '完成': 3,
            },



            // -------- CAN BE INJECTED --------------
            // 1 -> Lvl 1
            // 4 -> Lvl 4
            Customer_RankEnum: {
                'Lvl 1': 1,
                'Lvl 2': 2,
                'Lvl 3': 3,
                'Lvl 4': 4,
            },
            // ---------------------------------------

        },

        Language: "zh-cn"
    });
}(String, angular));                    //end pack