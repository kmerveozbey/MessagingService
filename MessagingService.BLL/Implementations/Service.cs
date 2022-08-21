using AutoMapper;
using MessagingService.DAL;
using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BLL.Implementations
{
    public class Service<TViewModel, TModel, Id> : IService<TViewModel, Id>
        where TViewModel : class, new()
        where TModel : class, new()
    {
        private readonly IRepository<TModel, Id> _repo;
        private readonly IMapper _mapper;
        private readonly string _includeEntities;

        public Service(IMapper mapper, IRepository<TModel, Id> repo,
            string includeEntities = null)
        {
            _mapper = mapper;
            _repo = repo;
            _includeEntities = includeEntities;
        }

        public IDataResult<TViewModel> Add(TViewModel model)
        {
            try
            {
                TModel tmodel = _mapper.Map<TViewModel, TModel>(model);
                bool result = _repo.Add(tmodel);
                TViewModel dataModel = _mapper.Map<TModel, TViewModel>(tmodel);

                return result ?
                    new DataResult<TViewModel>(true, "Ekleme işlemi başarılı!", dataModel)
                    : new DataResult<TViewModel>(false, "DİKKAT! Ekleme işlemi başarısız!", dataModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IResult Delete(TViewModel model)
        {
            try
            {
                TModel mdl = _mapper.Map<TViewModel, TModel>(model);
                bool deleteResult = _repo.Delete(mdl);
                var mdl1 = _mapper.Map<TModel, TViewModel>(mdl);
                return deleteResult ? new DataResult<TViewModel>(true, "Silme işlemi başarılıdır!", mdl1)
                : new DataResult<TViewModel>(false, "DİKKAT: Silme işlemi başarısız oldu!", mdl1);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IDataResult<ICollection<TViewModel>> GetAll(Expression<Func<TViewModel, bool>> filter = null)
        {
            try
            {
                var fltr = _mapper.Map<Expression<Func<TViewModel, bool>>,
                    Expression<Func<TModel, bool>>>(filter);
                var data = _repo.GetAll(fltr, includeEntities: _includeEntities);

                ICollection<TViewModel> dataList =
                    _mapper.Map<IQueryable<TModel>, ICollection<TViewModel>>(data);

                return new DataResult<ICollection<TViewModel>>(true, $"{dataList.Count} adet kayıt gönderildi", dataList);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IDataResult<TViewModel> GetByConditions(Expression<Func<TViewModel, bool>> filter = null)
        {
            try
            {
                var fltr = _mapper.Map<Expression<Func<TViewModel, bool>>, Expression<Func<TModel, bool>>>(filter);
                var data = _repo.GetByConditions(fltr, _includeEntities);
                if (data == null)
                {
                    return new DataResult<TViewModel>(false, "Kayıt bulunamadı", null);
                }

                var mdl = _mapper.Map<TModel, TViewModel>(data);
                return new DataResult<TViewModel>(true, "Kayıt bulundu", mdl);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IDataResult<TViewModel> GetById(Id id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("HATA! id null olamaz!");
                }
                var data = _repo.GetById(id);
                if (data == null)
                {
                    throw new Exception("HATA! Kayıt bulunamadı!");
                }
                var mdl = _mapper.Map<TModel, TViewModel>(data.Result);
                return new DataResult<TViewModel>(true, $"{id} id'li kayıt bulundu!", mdl);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IResult Update(TViewModel model)
        {
            try
            {
                TModel mdl = _mapper.Map<TViewModel, TModel>(model);                
                var updateResult = _repo.Update(mdl);
                var mdl1 = _mapper.Map<TModel, TViewModel>(mdl);
                return updateResult ?
    new DataResult<TViewModel>(true, "Güncelleme işlemi başarılı!", mdl1)
    : new DataResult<TViewModel>(false, "DİKKAT! Güncelleme işlemi başarısız!", mdl1);

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

