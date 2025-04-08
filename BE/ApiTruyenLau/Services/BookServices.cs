﻿using ApiTruyenLau.Services.Interfaces;
using DataConnecion.MongoObjects;
using Newtonsoft.Json;
using Item = ApiTruyenLau.Objects.Generics.Items;
using MGDBs = DataConnecion.MongoObjects.CommonObjects;
using User = ApiTruyenLau.Objects.Generics.Users;
using ItemCvt = ApiTruyenLau.Objects.Converters.Items;
using Newtonsoft.Json.Serialization;
using ApiTruyenLau.Objects.Converters.Items;
using ZstdSharp;


namespace ApiTruyenLau.Services
{
	public class BookServices : IBookServices
	{
		private readonly IConfiguration _configuration;
		private MGDBs _DB;
		private readonly Type[] typeColection = new Type[] { typeof(Item.Book) };
		public BookServices(IConfiguration configuration)
		{
			_configuration = configuration;
			this._DB = new MGDBs();
			var mongoDBSettings = _configuration.GetSection("MongoDB").Get<MongoDBSettings>();
			this._DB = this._DB.AddMongoDBSrv(mongoDBSettings?.ConnectionString!).AddMongoDBCollections(this.typeColection);
			//this._DB.GetMongoDBEntity(typeof(User.Client)).Indexs();
		}

		#region Phần intro sách
		/// <summary>
		/// Lúc này là hiện ra một số ô trưng bày sách rồi nhấn vô là nó gửi về bookId 
		/// Lúc này thì tìm Id rồi đánh về IntroBookPartCvt thôi 
		/// </summary>
		/// <param name="bookId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<ItemCvt.IntroBookPartCvt> GetIntroById(string bookId)
		{
			try
			{
				var findedBookObjs = await _DB.GetMongoDBEntity(typeof(Item.Book)).FindObjects(new Dictionary<string, string>()
				{
					{ nameof(Item.Book.Id), bookId}
				});
				if (findedBookObjs != null && findedBookObjs.Count > 0)
				{
					var settings = new JsonSerializerSettings
					{
						MissingMemberHandling = MissingMemberHandling.Ignore,
						SerializationBinder = new MySerializationBinderBook()
					};
					Item.Book? findedBook = findedBookObjs
						.Select(obj => JsonConvert.DeserializeObject<Item.Book>(JsonConvert.SerializeObject(obj), settings))
						.ToList().ElementAt(0);
					return findedBook?.ToIntroBookPartCvt()!;
				}
				throw new Exception($"không có quyển nào Id là {bookId}");
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		/// <summary>
		/// Theo lsy thuyết thì nó phân tích hành vi người dùng và trả về một số bản intro tương ứng với sở thích 
		/// {Còn hiện tại mấy bố làm Data lâu VL nên thôi cái này tạm thời bỏ qua}
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<ItemCvt.IntroBookPartCvt> GetIntros(string userId) /////////////////////////////// データをすばやく作成します。
		{
			try
			{
				// Đoạn trên đây lấy Data từ hàm phân tích hành vi các thứ rồi trả về thể loại hay gì đó 
				// Nếu nhiều hơn một thể loại thì chia thành nhiều Dict hoặc dùng 

				var findedBookObjs = await _DB.GetMongoDBEntity(typeof(Item.Book)).FindObjects(
					new KeyValuePair<string, List<string>>(
						nameof(Item.Book.Genre),
						new List<string>() { "Truyện tranh", "Truyện tranh" }
					)
				);
				throw new Exception("Chưa làm xong");
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
		#endregion Phần intro sách

		#region Phần tạo sách
		public async Task<bool> CreateBooks(List<ItemCvt.BookCreaterCvt> bookCreaterCvts)
		{
			try
			{
				/*IEnumerable<Item.Book> books = */
				bookCreaterCvts.Select(bookCreaterCvt =>
				{
					return bookCreaterCvt.ToBookCreaterCvt();
				})
				.AsParallel()
				.ForAll(async book =>
					await _DB.GetMongoDBEntity(typeof(Item.Book)).AddMongoDBEntity(book)
				);
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
		#endregion Phần tạo sách
	}

	public class MySerializationBinderBook : ISerializationBinder
	{
		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null!;
			typeName = serializedType.Name;
		}

		public Type BindToType(string assemblyName, string typeName)
		{
			return Type.GetType(typeName);
		}
	}
}
