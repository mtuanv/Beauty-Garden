(function ($) {
    "user strict";
    $.fn.Dqdt_CountDown = function (options) {
        return this.each(function () {
            new $.Dqdt_CountDown(this, options);
        });
    }
    $.Dqdt_CountDown = function (obj, options) {
        this.options = $.extend({
            autoStart: true,
            LeadingZero: true,
            DisplayFormat: "<div><span>%%D%%</span> Days</div><div><span>%%H%%</span> Hours</div><div><span>%%M%%</span> Mins</div><div><span>%%S%%</span> Secs</div>",
            FinishMessage: "",
            CountActive: true,
            TargetEndDate: null,
            TargetStartDate: null
        }, options || {});
        if (this.options.TargetEndDate == null || this.options.TargetEndDate == '' || this.options.TargetStartDate == '' || this.options.TargetStartDate == '') {
            return;
        }
        this.timer = null;
        this.element = obj;
        this.CountStepper = -1;
        this.CountStepper = Math.ceil(this.CountStepper);
        this.SetTimeOutPeriod = (Math.abs(this.CountStepper) - 1) * 1000 + 990;
        var dthen = new Date(this.options.TargetEndDate);
        var dnow = new Date();
        if (this.CountStepper > 0) {
            ddiff = new Date(dnow - dthen);
        }
        else {
            ddiff = new Date(dthen - dnow);
        }
        gsecs = Math.floor(ddiff.valueOf() / 1000);

        this.CountBack(gsecs, this);
    };
    $.Dqdt_CountDown.fn = $.Dqdt_CountDown.prototype;
    $.Dqdt_CountDown.fn.extend = $.Dqdt_CountDown.extend = $.extend;
    $.Dqdt_CountDown.fn.extend({
        calulatePercent: function (dayEnd, dayStart) {
            var dnowkilo = new Date();
            var dend = new Date(dayEnd);
            var dstart = new Date(dayStart);

            var rsd = new Date(dend - dnowkilo);
            var rs = new Date(dend - dstart);
            var gd = Math.floor(rsd.valueOf() / 1000);// ? %
            var gdff = Math.floor(rs.valueOf() / 1000);//100%

            if (rsd > 0) {
                var percent = (gd * 100) / gdff;
                return Math.round(percent);
            } else {
                return 0;
            }
        },
        calculateDate: function (secs, num1, num2) {
            var s = ((Math.floor(secs / num1)) % num2).toString();
            if (this.options.LeadingZero && s.length < 2) {
                s = "0" + s;
            }
            return "<b>" + s + "</b>";
        },
        CountBack: function (secs, self) {
            if (secs < 0) {
                self.element.innerHTML = '<div class="lof-labelexpired"> ' + self.options.FinishMessage + "</div>";
                return;
            }
            clearInterval(self.timer);
            DisplayStr = self.options.DisplayFormat.replace(/%%D%%/g, self.calculateDate(secs, 86400, 100000));
            DisplayStr = DisplayStr.replace(/%%H%%/g, self.calculateDate(secs, 3600, 24));
            DisplayStr = DisplayStr.replace(/%%M%%/g, self.calculateDate(secs, 60, 60));
            DisplayStr = DisplayStr.replace(/%%S%%/g, self.calculateDate(secs, 1, 60));
            DisplayStr = DisplayStr.replace(/%%P%%/g, self.calulatePercent(self.options.TargetEndDate, self.options.TargetStartDate));
            self.element.innerHTML = DisplayStr;
            if (self.options.CountActive) {
                self.timer = null;
                self.timer = setTimeout(function () {
                    self.CountBack((secs + self.CountStepper), self);
                }, (self.SetTimeOutPeriod));
            }
        }
    });
    $(document).ready(function () {
        loadAgain();
    });

    function loadAgain() {
        $('[data-countdown="countdown"]').each(function (index, el) {
            var $this = $(this);
            var $date = $this.data('date').split("-");
            var $startdate = $this.data('start').split("-");
            var isactive = $this.data('active');
            $this.Dqdt_CountDown({
                TargetEndDate: $date[0] + "/" + $date[1] + "/" + $date[2] + " " + $date[3] + ":" + $date[4] + ":" + $date[5],
                TargetStartDate: $startdate[0] + "/" + $startdate[1] + "/" + $startdate[2] + " " + $startdate[3] + ":" + $startdate[4] + ":" + $startdate[5],
                DisplayFormat: "<div class=\"ct-view\"><div class=\"block-timer\"><div class=\"cl-timer\">Còn lại <p>%%D%%</p> Ngày <p>%%H%%</p>:<p>%%M%%</p>:<p>%%S%%</p></div><div class=\"cl-perc\" style=\"width:%%P%%%\"></div></div><div class=\"pc\"><span>%%P%%%</span></div></div>",
                FinishMessage: "",
                CountActive: isactive
            });
        });
    }
})(jQuery);