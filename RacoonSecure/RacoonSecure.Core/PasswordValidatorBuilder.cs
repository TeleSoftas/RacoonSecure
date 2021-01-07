using RacoonSecure.Core.Settings;

namespace RacoonSecure.Core
{
    public class PasswordValidatorBuilder
    {
        private NistSettings _nistSettings;
        private CommonPasswordCheckSettings _commonPasswordCheckSettings;
        private BloomFilterSettings _bloomFilterSettings;

        public PasswordValidatorBuilder()
        {
            _nistSettings = new NistSettings();
            _commonPasswordCheckSettings = new CommonPasswordCheckSettings();
            _bloomFilterSettings = new BloomFilterSettings();
        }
        
        public PasswordValidatorBuilder UseNistGuidelines(NistSettings settings = null)
        {
            if(settings != null)
                _nistSettings = settings;
            
            _nistSettings.IsEnabled = true;
            return this;
        }
        
        public PasswordValidatorBuilder UseCommonPasswordCheck(CommonPasswordCheckSettings settings = null)
        {
            if(settings != null)
                _commonPasswordCheckSettings = settings;
            
            _commonPasswordCheckSettings.IsEnabled = true;
            return this;
        }

        public PasswordValidatorBuilder UseBloomFilter(BloomFilterSettings settings = null)
        {
            if(settings != null)
                _bloomFilterSettings = settings;
            
            _bloomFilterSettings.IsEnabled = true;
            return this;
        }
        
        public PasswordValidator Build() 
            => new PasswordValidator(
                _nistSettings,
                _commonPasswordCheckSettings,
                _bloomFilterSettings);
    }
}